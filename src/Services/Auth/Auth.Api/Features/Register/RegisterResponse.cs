namespace Auth.Api.Features.Register;

public sealed record RegisterResponse(Guid UserId, string Email, string Role, string AccessToken);
