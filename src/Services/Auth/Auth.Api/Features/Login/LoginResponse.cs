namespace Auth.Api.Features.Login;

public sealed record LoginResponse(Guid UserId, string Email, string Role, string AccessToken);
