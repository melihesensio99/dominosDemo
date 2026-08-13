using BuildingBlocks.Security;
using FluentValidation;
using Grpc.Net.Client;
using Inventory.Contracts.Grpc;
using StackExchange.Redis;

namespace Basket.Api.Infrastructure.Configuration;

public static class BasketServiceCollectionExtensions
{
    public static IServiceCollection AddBasketModule(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(BasketServiceCollectionExtensions).Assembly));
        services.AddValidationBehavior();
        services.AddValidatorsFromAssembly(typeof(BasketServiceCollectionExtensions).Assembly);
        services.AddJwtAuthentication(configuration);

        var redisConnectionString = configuration.GetConnectionString("Redis")
            ?? throw new InvalidOperationException("ConnectionStrings:Redis is missing.");

        var catalogApiUrl = configuration["CatalogApi:Url"]
            ?? throw new InvalidOperationException("CatalogApi:Url is missing.");

        var inventoryGrpcUrl = configuration["InventoryGrpc:Url"]
            ?? throw new InvalidOperationException("InventoryGrpc:Url is missing.");

        services.AddSingleton<IConnectionMultiplexer>(_ =>
            ConnectionMultiplexer.Connect(redisConnectionString));

        services.AddSingleton<IBasketRepository, RedisBasketRepository>();

        services.AddHttpClient<ICatalogProductClient, CatalogProductClient>(client =>
        {
            client.BaseAddress = new Uri(catalogApiUrl);
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

            return GrpcChannel.ForAddress(inventoryGrpcUrl, channelOptions);
        });

        services.AddSingleton(sp =>
            new InventoryStockService.InventoryStockServiceClient(sp.GetRequiredService<GrpcChannel>()));
        services.AddSingleton<IInventoryStockClient, InventoryGrpcStockClient>();

        return services;
    }
}
