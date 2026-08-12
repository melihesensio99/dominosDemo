using FluentValidation;
using Inventory.Api.Consumers;
using Inventory.Api.Features.Reservations;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Api.Infrastructure.Configuration;

public static class InventoryServiceCollectionExtensions
{
    public static IServiceCollection AddInventoryModule(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(InventoryServiceCollectionExtensions).Assembly));
        services.AddValidatorsFromAssembly(typeof(InventoryServiceCollectionExtensions).Assembly);
        services.AddValidationBehavior();

        var connectionString = configuration.GetConnectionString("InventoryDb")
            ?? throw new InvalidOperationException("ConnectionStrings:InventoryDb is missing.");

        services.AddDbContext<InventoryDbContext>(options => options.UseNpgsql(connectionString));
        services.AddScoped<IStockRepository, EfStockRepository>();
        services.AddScoped<StockReservationService>();
        services.AddGrpc();

        services.AddMassTransit(x =>
        {
            x.AddConsumer<ProductCreatedConsumer>();
            x.AddConsumer<OrderCancelledConsumer>();
            x.AddConsumer<OrderStatusChangedConsumer>();
            x.SetEndpointNameFormatter(new KebabCaseEndpointNameFormatter("inventory", false));

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

                cfg.UseMessageRetry(retry => retry.Intervals(
                    TimeSpan.FromSeconds(2),
                    TimeSpan.FromSeconds(5),
                    TimeSpan.FromSeconds(10)));

                cfg.ConfigureEndpoints(context);
            });
        });

        return services;
    }
}
