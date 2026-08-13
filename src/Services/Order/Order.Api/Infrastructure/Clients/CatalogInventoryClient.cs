using System.Net;
using System.Net.Http.Json;
using BuildingBlocks.Resilience;
using Order.Api.Abstractions;

namespace Order.Api.Infrastructure.Clients;

public sealed class CatalogInventoryClient(HttpClient httpClient) : ICatalogInventoryClient
{
    public async Task<CatalogInventoryProduct?> GetProductAsync(
        string productId,
        CancellationToken cancellationToken)
    {
        try
        {
            var response = await ResilienceExecutor.ExecuteAsync(
                async attemptToken => await httpClient.GetAsync($"products/{productId}", attemptToken),
                cancellationToken,
                shouldRetry: exception => exception is HttpRequestException or TimeoutException,
                timeout: TimeSpan.FromSeconds(10));

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return null;
            }

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
        catch (HttpRequestException)
        {
            return null;
        }
        catch (TimeoutException)
        {
            return null;
        }
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
