using Basket.Api.Features.Clear;
using Basket.Api.Features.Get;
using Basket.Api.Features.Items.Add;
using Basket.Api.Features.Items.Remove;
using Basket.Api.Features.Items.Update;
using FluentValidation;
using Grpc.Net.Client;
using Inventory.Contracts.Grpc;
using StackExchange.Redis;

namespace Basket.Api.Features;

public static class BasketModule
{
    public static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(BasketApiAssemblyMarker).Assembly));
        services.AddValidationBehavior();
        services.AddValidatorsFromAssembly(typeof(BasketApiAssemblyMarker).Assembly);

        services.AddSingleton<IConnectionMultiplexer>(_ =>
            ConnectionMultiplexer.Connect(configuration.GetConnectionString("Redis") ?? "localhost:6379"));

        services.AddSingleton<IBasketRepository, RedisBasketRepository>();

        services.AddSingleton(_ =>
            GrpcChannel.ForAddress(configuration["InventoryGrpc:Url"] ?? "http://localhost:5083"));
        services.AddSingleton(sp =>
            new InventoryStockService.InventoryStockServiceClient(sp.GetRequiredService<GrpcChannel>()));
        services.AddSingleton<IInventoryStockClient, InventoryGrpcStockClient>();
    }

    public static IEndpointRouteBuilder MapBasketEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGetBasketEndpoint();
        app.MapAddBasketItemEndpoint();
        app.MapUpdateBasketItemQuantityEndpoint();
        app.MapRemoveBasketItemEndpoint();
        app.MapClearBasketEndpoint();
        return app;
    }
}
