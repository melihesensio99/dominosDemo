using System.Security.Claims;
using System.Text;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using Notification.Api.Abstractions.Realtime;
using Notification.Api.Consumers;
using Notification.Api.Infrastructure;
using Notification.Api.Infrastructure.Realtime;

namespace Notification.Api.Features;

public static class NotificationModule
{
    public static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddCors(options =>
        {
            options.AddPolicy("RealtimeClients", policy =>
            {
                policy
                    .SetIsOriginAllowed(_ => true)
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
            });
        });

        ConfigureAuthentication(services, configuration);
        ConfigureMongoDb(services, configuration);

        services.AddSignalR();
        services.AddSingleton<IUserIdProvider, SubClaimUserIdProvider>();
        services.AddScoped<IRealtimeNotificationPublisher, SignalRNotificationPublisher>();

        services.AddMassTransit(x =>
        {
            x.AddConsumer<StockChangedConsumer>();
            x.AddConsumer<OrderCreatedConsumer>();
            x.AddConsumer<OrderCancelledConsumer>();
            x.AddConsumer<OrderStatusChangedConsumer>();
            x.SetKebabCaseEndpointNameFormatter();

            x.UsingRabbitMq((context, cfg) =>
            {
                var host = configuration["RabbitMq:Host"] ?? "localhost";
                var username = configuration["RabbitMq:Username"] ?? "guest";
                var password = configuration["RabbitMq:Password"] ?? "guest";

                cfg.Host(host, "/", h =>
                {
                    h.Username(username);
                    h.Password(password);
                });

                cfg.UseMessageRetry(retry =>
                {
                    retry.Intervals(
                        TimeSpan.FromSeconds(2),
                        TimeSpan.FromSeconds(5),
                        TimeSpan.FromSeconds(10));
                });

                cfg.ConfigureEndpoints(context);
            });
        });
    }

    public static IEndpointRouteBuilder MapNotificationEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/notifications", async (MongoNotificationStore store, CancellationToken cancellationToken) =>
        {
            var items = await store.GetAllAsync(cancellationToken);
            return Results.Ok(new { items });
        });

        app.MapPost("/notifications", async (
            CreateNotificationRequest request,
            MongoNotificationStore store,
            CancellationToken cancellationToken) =>
        {
            var notification = await store.AddAsync(
                request.RecipientId,
                request.Message,
                cancellationToken: cancellationToken);

            return Results.Accepted($"/notifications/{notification.Id}", notification);
        });

        app.MapGet("/notifications/{id}", async (
            string id,
            MongoNotificationStore store,
            CancellationToken cancellationToken) =>
        {
            return await store.GetByIdAsync(id, cancellationToken) is { } notification
                ? Results.Ok(notification)
                : Results.NotFound(new { error = "notification-not-found", id });
        });

        app.MapHub<NotificationHub>("/hubs/notifications");
        return app;
    }

    private static void ConfigureMongoDb(IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<NotificationMongoOptions>(configuration.GetSection("MongoDb"));

        services.AddSingleton<IMongoClient>(serviceProvider =>
        {
            var options = serviceProvider.GetRequiredService<IOptions<NotificationMongoOptions>>().Value;
            return new MongoClient(options.ConnectionString);
        });

        services.AddSingleton<IMongoCollection<NotificationDocument>>(serviceProvider =>
        {
            var options = serviceProvider.GetRequiredService<IOptions<NotificationMongoOptions>>().Value;
            var client = serviceProvider.GetRequiredService<IMongoClient>();
            return client
                .GetDatabase(options.Database)
                .GetCollection<NotificationDocument>(options.Collection);
        });

        services.AddSingleton<MongoNotificationStore>();
    }

    private static void ConfigureAuthentication(IServiceCollection services, IConfiguration configuration)
    {
        var issuer = configuration["Jwt:Issuer"]
            ?? throw new InvalidOperationException("Jwt:Issuer is missing.");
        var audience = configuration["Jwt:Audience"]
            ?? throw new InvalidOperationException("Jwt:Audience is missing.");
        var signingKey = configuration["Jwt:SigningKey"]
            ?? throw new InvalidOperationException("Jwt:SigningKey is missing.");

        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var accessToken = context.Request.Query["access_token"];
                        if (!string.IsNullOrEmpty(accessToken)
                            && context.HttpContext.Request.Path.StartsWithSegments("/hubs/notifications"))
                        {
                            context.Token = accessToken;
                        }

                        return Task.CompletedTask;
                    },
                };

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = issuer,
                    ValidateAudience = true,
                    ValidAudience = audience,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey)),
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero,
                    NameClaimType = "sub",
                    RoleClaimType = ClaimTypes.Role,
                };
            });

        services.AddAuthorization();
    }
}

public sealed record CreateNotificationRequest(string RecipientId, string Message);
