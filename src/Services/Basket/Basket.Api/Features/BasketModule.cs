using Basket.Api.Features.Clear;
using Basket.Api.Features.Get;
using Basket.Api.Features.Items.Add;
using Basket.Api.Features.Items.Remove;
using Basket.Api.Features.Items.Update;
using BuildingBlocks.Security;
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
        services.AddJwtAuthentication(configuration);

        services.AddSingleton<IConnectionMultiplexer>(_ =>
            ConnectionMultiplexer.Connect(configuration.GetConnectionString("Redis") ?? "localhost:6379"));

        services.AddSingleton<IBasketRepository, RedisBasketRepository>();

        services.AddHttpClient<ICatalogProductClient, CatalogProductClient>(client =>
        {
            client.BaseAddress = new Uri(configuration["CatalogApi:Url"] ?? "http://localhost:5174/");
        });

        services.AddSingleton(_ =>
        {
            var channelOptions = new GrpcChannelOptions();

            if (configuration.GetValue<bool>("InventoryGrpc:AllowInvalidCertificate"))
            {
                channelOptions.HttpHandler = new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator,
                };
            }

            return GrpcChannel.ForAddress(
                configuration["InventoryGrpc:Url"] ?? "http://localhost:5083",
                channelOptions);
        });
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
