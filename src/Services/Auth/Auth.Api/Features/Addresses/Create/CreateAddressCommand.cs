using BuildingBlocks.Common;
using MediatR;

namespace Auth.Api.Features.Addresses.Create;

public sealed record CreateAddressCommand(
    Guid UserId,
    string Title,
    string Street,
    string District,
    string City,
    string PostalCode,
    string Country) : IRequest<Result<UserAddressResponse>>;
