using System.Text.Json;
using Inventory.Contracts.IntegrationEvents.Catalog;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Api.Infrastructure.Outbox;

public sealed class CatalogOutboxDispatcher(
    IServiceScopeFactory scopeFactory,
    ILogger<CatalogOutboxDispatcher> logger) : BackgroundService
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);
    private const int MaxRetries = 5;
    private static readonly TimeSpan LockTimeout = TimeSpan.FromMinutes(5);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = scopeFactory.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<CatalogDbContext>();
                var publishEndpoint = scope.ServiceProvider.GetRequiredService<IPublishEndpoint>();
                var now = DateTimeOffset.UtcNow;
                var lockCutoff = now - LockTimeout;

                var pendingMessages = await dbContext.OutboxMessages
                    .Where(message =>
                        message.ProcessedAt == null &&
                        message.FailedAt == null &&
                        (message.LockedAt == null || message.LockedAt < lockCutoff))
                    .OrderBy(message => message.OccurredAt)
                    .Take(20)
                    .ToListAsync(stoppingToken);

                foreach (var message in pendingMessages)
                {
                    message.LockedAt = now;
                    message.ProcessingAt = now;
                    message.RetryCount += 1;
                }

                await dbContext.SaveChangesAsync(stoppingToken);

                foreach (var message in pendingMessages)
                {
                    try
                    {
                        await PublishAsync(publishEndpoint, message, stoppingToken);
                        message.ProcessedAt = DateTimeOffset.UtcNow;
                        message.ProcessingAt = null;
                        message.LockedAt = null;
                        message.Error = null;
                    }
                    catch (Exception exception)
                    {
                        message.Error = exception.Message;
                        message.ProcessingAt = null;
                        message.LockedAt = null;

                        if (message.RetryCount >= MaxRetries)
                        {
                            message.FailedAt = DateTimeOffset.UtcNow;
                            logger.LogError(
                                exception,
                                "Catalog outbox message permanently failed after {RetryCount} attempts. Type: {Type}, MessageId: {MessageId}",
                                message.RetryCount,
                                message.Type,
                                message.Id);
                        }
                        else
                        {
                            logger.LogWarning(
                                exception,
                                "Catalog outbox message failed and will be retried. Attempt: {RetryCount}, Type: {Type}, MessageId: {MessageId}",
                                message.RetryCount,
                                message.Type,
                                message.Id);
                        }
                    }
                }

                await dbContext.SaveChangesAsync(stoppingToken);
            }
            catch (Exception exception)
            {
                logger.LogError(exception, "Catalog outbox polling failed; the dispatcher will retry.");
            }

            await Task.Delay(TimeSpan.FromSeconds(2), stoppingToken);
        }
    }

    private static Task PublishAsync(
        IPublishEndpoint publishEndpoint,
        OutboxMessage message,
        CancellationToken cancellationToken)
    {
        return message.Type switch
        {
            "catalog.product-created" => publishEndpoint.Publish(
                JsonSerializer.Deserialize<ProductCreatedIntegrationEvent>(message.Payload, JsonOptions)!,
                cancellationToken),
            "catalog.product-updated" => publishEndpoint.Publish(
                JsonSerializer.Deserialize<ProductUpdatedIntegrationEvent>(message.Payload, JsonOptions)!,
                cancellationToken),
            "catalog.product-deleted" => publishEndpoint.Publish(
                JsonSerializer.Deserialize<ProductDeletedIntegrationEvent>(message.Payload, JsonOptions)!,
                cancellationToken),
            _ => Task.CompletedTask,
        };
    }
}
