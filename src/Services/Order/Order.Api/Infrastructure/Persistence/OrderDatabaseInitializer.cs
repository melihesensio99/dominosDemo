using BuildingBlocks.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Order.Api.Infrastructure.Persistence;

public static class OrderDatabaseInitializer
{
    public static async Task InitializeOrderDatabaseAsync(
        this IServiceProvider services,
        CancellationToken cancellationToken = default)
    {
        using var scope = services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<OrderDbContext>();

        await dbContext.Database.MigrateOrEnsureCreatedAsync(cancellationToken);
    }
}
