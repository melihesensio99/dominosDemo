using BuildingBlocks.Behaviors;
using Catalog.Api.Infrastructure.Outbox;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Api.Infrastructure.Configuration;

public static class CatalogServiceCollectionExtensions
{
    public static IServiceCollection AddCatalogModule(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(CatalogServiceCollectionExtensions).Assembly));
        services.AddValidatorsFromAssembly(typeof(CatalogServiceCollectionExtensions).Assembly);
        services.AddValidationBehavior();

        var connectionString = configuration.GetConnectionString("CatalogDb")
            ?? throw new InvalidOperationException("ConnectionStrings:CatalogDb is missing.");

        services.AddDbContext<CatalogDbContext>(options => options.UseNpgsql(connectionString));
        services.AddScoped<IProductRepository, EfProductRepository>();
        services.AddScoped<ICategoryRepository, EfCategoryRepository>();
        services.AddHostedService<CatalogOutboxDispatcher>();

        return services;
    }
}
