using System.Net;
using System.Net.Http.Json;

namespace Basket.Api.Infrastructure;

public sealed class CatalogProductClient(HttpClient httpClient) : ICatalogProductClient
{
    public async Task<Result<CatalogProductSnapshot>> GetProductAsync(string productId, CancellationToken cancellationToken)
    {
        using var response = await httpClient.GetAsync($"products/{productId}", cancellationToken);
        if (response.IsSuccessStatusCode)
        {
            var product = await response.Content.ReadFromJsonAsync<CatalogProductResponse>(cancellationToken);
            if (product is null)
            {
                return Result<CatalogProductSnapshot>.Failure("catalog.invalid_response", "Catalog returned an invalid product response.");
            }

            return Result<CatalogProductSnapshot>.Success(new CatalogProductSnapshot(
                product.Id.ToString(),
                product.Name,
                product.Price,
                product.IsActive,
                product.InventoryTrackingType,
                product.InventoryKey,
                product.OptionGroups.Select(group => new CatalogOptionGroupSnapshot(
                    group.Id,
                    group.Name,
                    group.SelectionType,
                    group.IsRequired,
                    group.Options.Select(option => new CatalogOptionSnapshot(
                        option.Id,
                        group.Name,
                        option.Name,
                        option.PriceAdjustment,
                        option.InventoryKey,
                        option.IsDefault,
                        option.IsActive)).ToArray())).ToArray()));
        }

        return response.StatusCode == HttpStatusCode.NotFound
            ? Result<CatalogProductSnapshot>.NotFound("catalog.product_not_found", "Product was not found.")
            : Result<CatalogProductSnapshot>.Failure("catalog.unavailable", "Catalog could not be reached.");
    }

    private sealed record CatalogProductResponse(
        Guid Id,
        string Name,
        decimal Price,
        bool IsActive,
        string InventoryTrackingType,
        string? InventoryKey,
        IReadOnlyList<CatalogOptionGroupResponse> OptionGroups);

    private sealed record CatalogOptionGroupResponse(
        Guid Id,
        string Name,
        string SelectionType,
        bool IsRequired,
        IReadOnlyList<CatalogOptionResponse> Options);

    private sealed record CatalogOptionResponse(
        Guid Id,
        string Name,
        decimal PriceAdjustment,
        string? InventoryKey,
        bool IsDefault,
        bool IsActive,
        int DisplayOrder);
}
