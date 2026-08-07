namespace Auth.Api.Application.Abstractions.Security;

public interface ITokenService
{
    string CreateToken(Guid userId, string email, string role);
}
