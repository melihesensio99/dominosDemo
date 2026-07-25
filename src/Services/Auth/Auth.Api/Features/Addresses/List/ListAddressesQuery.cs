using BuildingBlocks.Common;
using MediatR;

namespace Auth.Api.Features.Addresses.List;

public sealed record ListAddressesQuery(Guid UserId) : IRequest<Result<IReadOnlyList<UserAddressResponse>>>;
