using FluentValidation;
using Grpc.Net.Client;
using Inventory.Contracts.Grpc;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Order.Api.Abstractions;
using Order.Api.Infrastructure.Clients;
using Order.Api.Infrastructure.Outbox;
using Order.Api.Infrastructure.Persistence;
using System.Security.Claims;
using System.Text;

namespace Order.Api.Infrastructure.Configuration;

public static class OrderServiceCollectionExtensions
{
    public static IServiceCollection AddOrderModule(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddCors(options =>
        {
            options.AddPolicy("AdminPanel", policy =>
            {
                policy
                    .SetIsOriginAllowed(_ => true)
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
            });
        });

        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(OrderServiceCollectionExtensions).Assembly));
        services.AddValidationBehavior();
        services.AddValidatorsFromAssembly(typeof(OrderServiceCollectionExtensions).Assembly);

        var connectionString = configuration.GetConnectionString("OrderDb")
            ?? throw new InvalidOperationException("ConnectionStrings:OrderDb is missing.");

        services.AddDbContext<OrderDbContext>(options => options.UseNpgsql(connectionString));
        services.AddScoped<IOrderRepository, EfOrderRepository>();
        services.AddHttpClient<ICatalogInventoryClient, CatalogInventoryClient>(client =>
        {
            client.BaseAddress = new Uri(configuration["CatalogApi:Url"] ?? "http://localhost:5174/");
        });

        services.AddSingleton(_ => GrpcChannel.ForAddress(
            configuration["InventoryGrpc:Url"] ?? "http://localhost:5142"));
        services.AddSingleton(serviceProvider =>
            new InventoryStockService.InventoryStockServiceClient(
                serviceProvider.GetRequiredService<GrpcChannel>()));
        services.AddSingleton<IOrderStockClient, OrderGrpcStockClient>();
        services.AddHostedService<OrderOutboxDispatcher>();

        var jwtIssuer = configuration["Jwt:Issuer"]
            ?? throw new InvalidOperationException("Jwt:Issuer is missing.");
        var jwtAudience = configuration["Jwt:Audience"]
            ?? throw new InvalidOperationException("Jwt:Audience is missing.");
        var jwtSigningKey = configuration["Jwt:SigningKey"]
            ?? throw new InvalidOperationException("Jwt:SigningKey is missing.");

        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = jwtIssuer,
                    ValidateAudience = true,
                    ValidAudience = jwtAudience,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSigningKey)),
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero,
                    NameClaimType = "sub",
                    RoleClaimType = ClaimTypes.Role,
                };
            });

        services.AddAuthorization(options =>
        {
            options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
        });

        services.AddMassTransit(x =>
        {
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
            });
        });

        return services;
    }
}
