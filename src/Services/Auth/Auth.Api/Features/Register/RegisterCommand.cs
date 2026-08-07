using BuildingBlocks.Common;
using MediatR;

namespace Auth.Api.Features.Register;

public sealed record RegisterCommand(string Email, string Password, string ConfirmPassword) : IRequest<Result<RegisterResponse>>;
