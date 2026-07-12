using BuildingBlocks.Common;
using MediatR;

namespace Auth.Api.Features.Login;

public sealed record LoginCommand(string Email, string Password) : IRequest<Result<LoginResponse>>;
