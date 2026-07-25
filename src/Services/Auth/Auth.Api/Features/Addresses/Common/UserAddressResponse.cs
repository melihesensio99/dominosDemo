namespace Auth.Api.Features.Addresses;

public sealed record UserAddressResponse(
    Guid Id,
    string Title,
    string Street,
    string District,
    string City,
    string PostalCode,
    string Country);

public static class UserAddressMapper
{
    public static UserAddressResponse ToResponse(this UserAddress address) => new(
        address.Id,
        address.Title,
        address.Street,
        address.District,
        address.City,
        address.PostalCode,
        address.Country);
}
