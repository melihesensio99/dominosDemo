using Catalog.Api.Features.Categories;
using Catalog.Api.Features.Products;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Api.Features;

public static class CatalogModule
{
    public static IServiceCollection ConfigureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(CatalogModule).Assembly));
        services.AddValidatorsFromAssembly(typeof(CatalogModule).Assembly);
        services.AddValidationBehavior();

        var connectionString = configuration.GetConnectionString("CatalogDb")
            ?? throw new InvalidOperationException("ConnectionStrings:CatalogDb is missing.");

        services.AddDbContext<CatalogDbContext>(options => options.UseNpgsql(connectionString));
        services.AddScoped<IProductRepository, EfProductRepository>();
        services.AddScoped<ICategoryRepository, EfCategoryRepository>();

        return services;
    }

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
