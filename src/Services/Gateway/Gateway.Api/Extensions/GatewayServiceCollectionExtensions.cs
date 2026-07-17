using Gateway.Api.Proxy;

namespace Gateway.Api.Extensions;

public static class GatewayServiceCollectionExtensions
{
    public static IServiceCollection AddGatewayProxy(this IServiceCollection services)
    {
        services.AddHttpClient();
        services.AddSingleton<GatewayProxyExecutor>();

        return services;
    }
}
