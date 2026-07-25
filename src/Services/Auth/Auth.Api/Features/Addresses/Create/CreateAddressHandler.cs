namespace Auth.Api.Features.Addresses.Create;

public sealed class CreateAddressHandler(IUserAddressRepository addressRepository)
    : IRequestHandler<CreateAddressCommand, Result<UserAddressResponse>>
{
    public async Task<Result<UserAddressResponse>> Handle(CreateAddressCommand request, CancellationToken cancellationToken)
    {
        var address = new UserAddress
        {
            UserId = request.UserId,
            Title = request.Title.Trim(),
            Street = request.Street.Trim(),
            District = request.District.Trim(),
            City = request.City.Trim(),
            PostalCode = request.PostalCode.Trim(),
            Country = request.Country.Trim(),
        };

        await addressRepository.AddAsync(address, cancellationToken);
        return Result<UserAddressResponse>.Success(address.ToResponse());
    }
}
