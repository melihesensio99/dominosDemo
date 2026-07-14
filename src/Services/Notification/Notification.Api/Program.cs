using MassTransit;
using Notification.Api;
using Notification.Api.Consumers;
using Notification.Api.Infrastructure;
using MongoDB.Driver;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<NotificationMongoOptions>(builder.Configuration.GetSection("MongoDb"));

builder.Services.AddSingleton<IMongoClient>(sp =>
{
    var options = sp.GetRequiredService<IOptions<NotificationMongoOptions>>().Value;
    return new MongoClient(options.ConnectionString);
});

builder.Services.AddSingleton<IMongoCollection<NotificationDocument>>(sp =>
{
    var options = sp.GetRequiredService<IOptions<NotificationMongoOptions>>().Value;
    var client = sp.GetRequiredService<IMongoClient>();
    var database = client.GetDatabase(options.Database);
    return database.GetCollection<NotificationDocument>(options.Collection);
});

builder.Services.AddSingleton<MongoNotificationStore>();

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<StockChangedConsumer>();
    x.AddConsumer<OrderCreatedConsumer>();
    x.AddConsumer<OrderCancelledConsumer>();
    x.SetKebabCaseEndpointNameFormatter();

    x.UsingRabbitMq((context, cfg) =>
    {
        var host = builder.Configuration["RabbitMq:Host"] ?? "localhost";
        var username = builder.Configuration["RabbitMq:Username"] ?? "guest";
        var password = builder.Configuration["RabbitMq:Password"] ?? "guest";

        cfg.Host(host, "/", h =>
        {
            h.Username(username);
            h.Password(password);
        });

        cfg.ConfigureEndpoints(context);
    });
});

var app = builder.Build();

app.MapGet("/notifications", async (MongoNotificationStore store, CancellationToken cancellationToken) =>
{
    var items = await store.GetAllAsync(cancellationToken);
    return Results.Ok(new { items });
});

app.MapPost("/notifications", async (CreateNotificationRequest request, MongoNotificationStore store, CancellationToken cancellationToken) =>
{
    var notification = await store.AddAsync(request.RecipientId, request.Message, cancellationToken: cancellationToken);
    return Results.Accepted($"/notifications/{notification.Id}", notification);
});

app.MapGet("/notifications/{id}", async (string id, MongoNotificationStore store, CancellationToken cancellationToken) =>
{
    return await store.GetByIdAsync(id, cancellationToken) is { } notification
        ? Results.Ok(notification)
        : Results.NotFound(new { error = "notification-not-found", id });
});

app.Run();

internal sealed record CreateNotificationRequest(string RecipientId, string Message);
