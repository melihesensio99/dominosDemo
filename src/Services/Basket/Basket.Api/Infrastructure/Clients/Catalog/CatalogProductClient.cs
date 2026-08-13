using System.Net.Http.Json;
using BuildingBlocks.Resilience;
using Basket.Api.Infrastructure.Clients.Catalog.Contracts;

namespace Basket.Api.Infrastructure.Clients.Catalog;

public sealed class CatalogProductClient(HttpClient httpClient) : ICatalogProductClient
{
    public async Task<Result<CatalogProductSnapshot>> GetProductAsync(string productId, CancellationToken cancellationToken)
    {
        try
        {
            return await ResilienceExecutor.ExecuteAsync(async attemptToken =>
            {
                using var response = await httpClient.GetAsync($"products/{productId}", attemptToken);
                if (response.IsSuccessStatusCode)
                {
                    var product = await response.Content.ReadFromJsonAsync<CatalogProductResponse>(attemptToken);
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

                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return Result<CatalogProductSnapshot>.NotFound("catalog.product_not_found", "Product was not found.");
                }

                if (response.StatusCode is System.Net.HttpStatusCode.RequestTimeout
                    or System.Net.HttpStatusCode.TooManyRequests
                    or System.Net.HttpStatusCode.BadGateway
                    or System.Net.HttpStatusCode.ServiceUnavailable
                    or System.Net.HttpStatusCode.GatewayTimeout)
                {
                    throw new HttpRequestException($"Catalog returned transient status {(int)response.StatusCode}.");
                }

                return Result<CatalogProductSnapshot>.Failure("catalog.unavailable", "Catalog could not be reached.");
            }, cancellationToken);
        }
        catch (HttpRequestException)
        {
            return Result<CatalogProductSnapshot>.Failure("catalog.unavailable", "Catalog could not be reached.");
        }
        catch (TimeoutException)
        {
            return Result<CatalogProductSnapshot>.Failure("catalog.unavailable", "Catalog request timed out.");
        }
    }

}
