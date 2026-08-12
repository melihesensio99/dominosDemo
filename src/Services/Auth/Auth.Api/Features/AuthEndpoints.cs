using Auth.Api.Features.Login;
using Auth.Api.Features.Register;

namespace Auth.Api.Features;

public static class AuthEndpoints
{
    public static IEndpointRouteBuilder MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapLoginEndpoint();
        app.MapRegisterEndpoint();
        app.MapAddressEndpoints();
        return app;
    }
}
