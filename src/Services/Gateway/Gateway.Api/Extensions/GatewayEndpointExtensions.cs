using Gateway.Api.Options;
using Gateway.Api.Proxy;
using Microsoft.Extensions.Options;

namespace Gateway.Api.Extensions;

public static class GatewayEndpointExtensions
{
    public static IEndpointRouteBuilder MapGatewayEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/services", (IOptions<GatewayOptions> options) => Results.Ok(new
        {
            service = Environment.GetEnvironmentVariable("SERVICE_NAME") ?? "gateway",
            downstream = options.Value.DownstreamServices,
        }));

        app.MapMethods("/proxy/{serviceName}/{**path}", new[] { "GET", "POST", "PUT", "PATCH", "DELETE" }, ProxyAsync);

        return app;
    }

    private static async Task ProxyAsync(
        HttpContext context,
        string serviceName,
        string? path,
        GatewayProxyExecutor proxyExecutor,
        CancellationToken cancellationToken)
    {
        await proxyExecutor.ProxyAsync(context, serviceName, path, cancellationToken);
    }
}
