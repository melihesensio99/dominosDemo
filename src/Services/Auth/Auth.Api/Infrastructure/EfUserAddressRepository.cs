using Microsoft.EntityFrameworkCore;

namespace Auth.Api.Infrastructure;

public sealed class EfUserAddressRepository(AuthDbContext dbContext) : IUserAddressRepository
{
    public async Task<IReadOnlyList<UserAddress>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken)
    {
        return await dbContext.UserAddresses
            .Where(x => x.UserId == userId)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(UserAddress address, CancellationToken cancellationToken)
    {
        dbContext.UserAddresses.Add(address);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> DeleteAsync(Guid userId, Guid addressId, CancellationToken cancellationToken)
    {
        var address = await dbContext.UserAddresses
            .FirstOrDefaultAsync(x => x.UserId == userId && x.Id == addressId, cancellationToken);

        if (address is null)
        {
            return false;
        }

        dbContext.UserAddresses.Remove(address);
        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }
}
