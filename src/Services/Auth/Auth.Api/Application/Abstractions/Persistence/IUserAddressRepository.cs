namespace Auth.Api.Application.Abstractions.Persistence;

public interface IUserAddressRepository
{
    Task<IReadOnlyList<UserAddress>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken);

    Task AddAsync(UserAddress address, CancellationToken cancellationToken);

    Task<bool> DeleteAsync(Guid userId, Guid addressId, CancellationToken cancellationToken);
}
