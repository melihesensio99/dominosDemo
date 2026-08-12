using Catalog.Api.Features.Categories;
using Catalog.Api.Features.Products;

namespace Catalog.Api.Features;

public static class CatalogEndpoints
{
    public static IEndpointRouteBuilder MapCatalogEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGetProductsEndpoint();
        app.MapGetProductByIdEndpoint();
        app.MapCreateProductEndpoint();
        app.MapUpdateProductEndpoint();
        app.MapDeleteProductEndpoint();

        app.MapGetCategoriesEndpoint();
        app.MapCategoryDetailsEndpoint();
        app.MapCreateCategoryEndpoint();
        app.MapUpdateCategoryEndpoint();
        app.MapDeleteCategoryEndpoint();
        return app;
    }
}
