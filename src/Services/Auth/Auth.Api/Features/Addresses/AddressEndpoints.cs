using Auth.Api.Features.Addresses.Create;
using Auth.Api.Features.Addresses.Delete;
using Auth.Api.Features.Addresses.List;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Auth.Api.Features.Addresses;

public static class AddressEndpoints
{
    public static IEndpointRouteBuilder MapAddressEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/auth/addresses").RequireAuthorization();

        group.MapGet("", async (ClaimsPrincipal user, ISender sender, CancellationToken cancellationToken) =>
        {
            if (!user.TryGetUserId(out var userId))
            {
                return Results.Unauthorized();
            }

            var result = await sender.Send(new ListAddressesQuery(userId), cancellationToken);
            return result.ToHttpResult();
        });

        group.MapPost("", async (CreateAddressRequest request, ClaimsPrincipal user, ISender sender, CancellationToken cancellationToken) =>
        {
            if (!user.TryGetUserId(out var userId))
            {
                return Results.Unauthorized();
            }

            var result = await sender.Send(new CreateAddressCommand(
                userId,
                request.Title,
                request.Street,
                request.District,
                request.City,
                request.PostalCode,
                request.Country), cancellationToken);
            return result.ToHttpResult();
        });

        group.MapDelete("/{addressId:guid}", async (Guid addressId, ClaimsPrincipal user, ISender sender, CancellationToken cancellationToken) =>
        {
            if (!user.TryGetUserId(out var userId))
            {
                return Results.Unauthorized();
            }

            var result = await sender.Send(new DeleteAddressCommand(userId, addressId), cancellationToken);
            return result.ToHttpResult();
        });

        return app;
    }
}

public sealed record CreateAddressRequest(
    string Title,
    string Street,
    string District,
    string City,
    string PostalCode,
    string Country);
