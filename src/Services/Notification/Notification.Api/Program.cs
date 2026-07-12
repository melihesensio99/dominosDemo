using MassTransit;
using Notification.Api;
using Notification.Api.Consumers;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<NotificationStore>();

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<StockChangedConsumer>();
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

app.MapGet("/", () => Results.Ok(new
{
    service = Environment.GetEnvironmentVariable("SERVICE_NAME") ?? "notification",
    status = "ok",
}));

app.MapGet("/health", () => Results.Ok(new
{
    service = Environment.GetEnvironmentVariable("SERVICE_NAME") ?? "notification",
    status = "ok",
}));

app.MapGet("/notifications", (NotificationStore store) => Results.Ok(new { items = store.GetAll() }));

app.MapPost("/notifications", (CreateNotificationRequest request, NotificationStore store) =>
{
    var notification = store.Add(request.RecipientId, request.Message);
    return Results.Accepted($"/notifications/{notification.Id}", notification);
});

app.MapGet("/notifications/{id}", (string id, NotificationStore store) =>
{
    return store.GetById(id) is { } notification
        ? Results.Ok(notification)
        : Results.NotFound(new { error = "notification-not-found", id });
});

app.Run();

internal sealed record CreateNotificationRequest(string RecipientId, string Message);
