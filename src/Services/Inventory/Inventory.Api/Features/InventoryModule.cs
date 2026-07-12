using FluentValidation;
using Inventory.Api.Features.Stock;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Api.Features;

public static class InventoryModule
{
    public static IServiceCollection ConfigureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(InventoryModule).Assembly));
        services.AddValidatorsFromAssembly(typeof(InventoryModule).Assembly);
        services.AddValidationBehavior();

        var connectionString = configuration.GetConnectionString("InventoryDb")
            ?? throw new InvalidOperationException("ConnectionStrings:InventoryDb is missing.");

        services.AddDbContext<InventoryDbContext>(options => options.UseNpgsql(connectionString));
        services.AddScoped<IStockRepository, EfStockRepository>();

        return services;
    }

    public static IEndpointRouteBuilder MapInventoryEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapStockEndpoints();
        return app;
    }
}
