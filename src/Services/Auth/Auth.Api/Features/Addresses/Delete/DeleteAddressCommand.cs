using BuildingBlocks.Common;
using MediatR;

namespace Auth.Api.Features.Addresses.Delete;

public sealed record DeleteAddressCommand(Guid UserId, Guid AddressId) : IRequest<Result>;
