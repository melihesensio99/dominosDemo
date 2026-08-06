using FluentValidation;
using MediatR;
using Order.Api.Abstractions;
using Order.Api.Infrastructure.Outbox;
using Order.Api.Infrastructure.Persistence;
using Order.Api.Features.Cancel;
using Order.Api.Features.Create;
using Order.Api.Features.Get;
using Order.Api.Features.GetByCustomer;
using Order.Api.Features.List;
using Order.Api.Features.UpdateStatus;
using Microsoft.EntityFrameworkCore;
using Grpc.Net.Client;
using Inventory.Contracts.Grpc;
using Order.Api.Infrastructure.Clients;

namespace Order.Api.Features;

public static class OrderModule
{
    public static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(OrderApiAssemblyMarker).Assembly));
        services.AddValidationBehavior();
        services.AddValidatorsFromAssembly(typeof(OrderApiAssemblyMarker).Assembly);

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
    }

    public static IEndpointRouteBuilder MapOrderEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapCreateOrderEndpoint();
        app.MapGetOrdersEndpoint();
        app.MapGetOrderEndpoint();
        app.MapGetMyOrdersEndpoint();
        app.MapGetCustomerOrdersEndpoint();
        app.MapCancelOrderEndpoint();
        app.MapUpdateOrderStatusEndpoint();
        return app;
    }
}
