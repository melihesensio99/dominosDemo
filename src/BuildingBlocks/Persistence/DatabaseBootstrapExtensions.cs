using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace BuildingBlocks.Persistence;

public static class DatabaseBootstrapExtensions
{
    public static Task MigrateDatabaseAsync(this DatabaseFacade database, CancellationToken cancellationToken = default)
    {
        return database.MigrateAsync(cancellationToken);
    }
}
