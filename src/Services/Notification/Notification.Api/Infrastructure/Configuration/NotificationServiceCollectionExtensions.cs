using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using Notification.Api.Abstractions.Realtime;
using Notification.Api.Consumers;
using Notification.Api.Infrastructure.Realtime;
using System.Security.Claims;
using System.Text;

namespace Notification.Api.Infrastructure.Configuration;

public static class NotificationServiceCollectionExtensions
{
    public static IServiceCollection AddNotificationModule(this IServiceCollection services, IConfiguration configuration)
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
            x.AddConsumer<LowStockDetectedConsumer>();
            x.AddConsumer<OrderCreatedConsumer>();
            x.AddConsumer<OrderCancelledConsumer>();
            x.AddConsumer<OrderStatusChangedConsumer>();
            x.SetEndpointNameFormatter(new KebabCaseEndpointNameFormatter("notification", false));

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

        return services;
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
                options.MapInboundClaims = false;
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
