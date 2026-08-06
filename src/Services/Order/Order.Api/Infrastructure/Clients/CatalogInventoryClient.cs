using System.Net.Http.Json;
using Order.Api.Abstractions;

namespace Order.Api.Infrastructure.Clients;

public sealed class CatalogInventoryClient(HttpClient httpClient) : ICatalogInventoryClient
{
    public async Task<CatalogInventoryProduct?> GetProductAsync(
        string productId,
        CancellationToken cancellationToken)
    {
        using var response = await httpClient.GetAsync($"products/{productId}", cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        var product = await response.Content.ReadFromJsonAsync<CatalogProductResponse>(cancellationToken);
        return product is null
            ? null
            : new CatalogInventoryProduct(
                product.Id.ToString(),
                product.InventoryTrackingType,
                product.InventoryKey,
                product.IsActive,
                product.Price,
                product.OptionGroups
                    .SelectMany(group => group.Options)
                    .Select(option => new CatalogInventoryOption(option.Id, option.InventoryKey, option.IsActive, option.PriceAdjustment))
                    .ToArray());
    }

    private sealed record CatalogProductResponse(
        Guid Id,
        string InventoryTrackingType,
        string? InventoryKey,
        bool IsActive,
        decimal Price,
        IReadOnlyList<CatalogOptionGroupResponse> OptionGroups);

    private sealed record CatalogOptionGroupResponse(IReadOnlyList<CatalogOptionResponse> Options);

    private sealed record CatalogOptionResponse(Guid Id, string? InventoryKey, bool IsActive, decimal PriceAdjustment);
}
