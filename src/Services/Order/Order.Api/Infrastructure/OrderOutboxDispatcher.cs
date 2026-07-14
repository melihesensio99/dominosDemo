using System.Text.Json;
using Inventory.Contracts.IntegrationEvents.Order;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

namespace Order.Api.Infrastructure;

public sealed class OrderOutboxDispatcher(IServiceScopeFactory scopeFactory) : BackgroundService
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
                var dbContext = scope.ServiceProvider.GetRequiredService<OrderDbContext>();
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
                        }
                    }
                }

                await dbContext.SaveChangesAsync(stoppingToken);
            }
            catch
            {
                // basit öğrenme projesi icin sessiz tekrar dene
            }

            await Task.Delay(TimeSpan.FromSeconds(2), stoppingToken);
        }
    }

    private static Task PublishAsync(IPublishEndpoint publishEndpoint, OutboxMessage message, CancellationToken cancellationToken)
    {
        return message.Type switch
        {
            "order.created" => publishEndpoint.Publish(
                JsonSerializer.Deserialize<OrderCreatedIntegrationEvent>(message.Payload, JsonOptions)!,
                cancellationToken),
            "order.cancelled" => publishEndpoint.Publish(
                JsonSerializer.Deserialize<OrderCancelledIntegrationEvent>(message.Payload, JsonOptions)!,
                cancellationToken),
            _ => Task.CompletedTask,
        };
    }
}
