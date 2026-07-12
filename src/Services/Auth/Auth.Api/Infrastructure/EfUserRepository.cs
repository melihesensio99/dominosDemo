using Auth.Api.Abstractions;
using Auth.Api.Domain;
using Microsoft.EntityFrameworkCore;

namespace Auth.Api.Infrastructure;

public sealed class EfUserRepository(AuthDbContext dbContext) : IUserRepository
{
    public async Task AddAsync(User user, CancellationToken cancellationToken)
    {
        dbContext.Users.Add(user);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken)
    {
        var normalized = email.Trim().ToLowerInvariant();
        return dbContext.Users.FirstOrDefaultAsync(x => x.Email == normalized, cancellationToken);
    }
}
