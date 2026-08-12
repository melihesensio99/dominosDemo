using Auth.Api.Domain;
using Auth.Api.Application.Abstractions.Security;
using BuildingBlocks.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Auth.Api.Infrastructure.Persistence;

public static class AuthDatabaseInitializer
{
    private const string AdminEmail = "admin@opsflow.ai";
    private const string AdminPassword = "P@ssw0rd123";
    private const string LegacyAdminPasswordHash = "231ECC7D178DA5F22983BC579599396D6C139A457987AE1EE0026D88432D6A72";

    public static async Task InitializeAuthDatabaseAsync(this IServiceProvider services, CancellationToken cancellationToken = default)
    {
        using var scope = services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AuthDbContext>();
        var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();

        await dbContext.Database.MigrateDatabaseAsync(cancellationToken);
        await SeedAdminUserAsync(dbContext, passwordHasher, cancellationToken);
    }

    private static async Task SeedAdminUserAsync(
        AuthDbContext dbContext,
        IPasswordHasher passwordHasher,
        CancellationToken cancellationToken)
    {
        var existingAdmin = await dbContext.Users
            .FirstOrDefaultAsync(x => x.Email == AdminEmail, cancellationToken);

        if (existingAdmin is not null)
        {
            if (existingAdmin.PasswordHash == LegacyAdminPasswordHash)
            {
                existingAdmin.PasswordHash = passwordHasher.Hash(AdminPassword);
                await dbContext.SaveChangesAsync(cancellationToken);
            }

            return;
        }

        dbContext.Users.Add(new User
        {
            Id = Guid.Parse("00000000-0000-0000-0000-000000000001"),
            Email = AdminEmail,
            PasswordHash = passwordHasher.Hash(AdminPassword),
            Role = "Admin",
            CreatedAt = DateTimeOffset.UtcNow,
        });

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
