namespace Auth.Api.Features.Addresses.List;

public sealed class ListAddressesHandler(IUserAddressRepository addressRepository)
    : IRequestHandler<ListAddressesQuery, Result<IReadOnlyList<UserAddressResponse>>>
{
    public async Task<Result<IReadOnlyList<UserAddressResponse>>> Handle(ListAddressesQuery request, CancellationToken cancellationToken)
    {
        var addresses = await addressRepository.GetByUserIdAsync(request.UserId, cancellationToken);
        return Result<IReadOnlyList<UserAddressResponse>>.Success(addresses.Select(x => x.ToResponse()).ToList());
    }
}
