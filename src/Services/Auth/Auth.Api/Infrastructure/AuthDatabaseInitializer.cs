using Auth.Api.Domain;
using BuildingBlocks.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Auth.Api.Infrastructure;

public static class AuthDatabaseInitializer
{
    public static async Task InitializeAuthDatabaseAsync(this IServiceProvider services, CancellationToken cancellationToken = default)
    {
        using var scope = services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AuthDbContext>();

        await dbContext.Database.MigrateOrEnsureCreatedAsync(cancellationToken);
        await SeedAdminUserAsync(dbContext, cancellationToken);
    }

    private static async Task SeedAdminUserAsync(AuthDbContext dbContext, CancellationToken cancellationToken)
    {
        if (await dbContext.Users.AnyAsync(x => x.Email == "admin@opsflow.ai", cancellationToken))
        {
            return;
        }

        dbContext.Users.Add(new User
        {
            Id = Guid.Parse("00000000-0000-0000-0000-000000000001"),
            Email = "admin@opsflow.ai",
            PasswordHash = "231ECC7D178DA5F22983BC579599396D6C139A457987AE1EE0026D88432D6A72",
            Role = "Admin",
            CreatedAt = DateTimeOffset.UtcNow,
        });

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
