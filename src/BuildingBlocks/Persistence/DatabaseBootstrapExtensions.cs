using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace BuildingBlocks.Persistence;

public static class DatabaseBootstrapExtensions
{
    public static async Task MigrateOrEnsureCreatedAsync(this DatabaseFacade database, CancellationToken cancellationToken = default)
    {
        if (database.GetMigrations().Any())
        {
            await database.MigrateAsync(cancellationToken);
            return;
        }

        await database.EnsureCreatedAsync(cancellationToken);
    }
}
