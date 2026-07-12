namespace Auth.Api.Abstractions.Application;

public interface ITokenService
{
    string CreateToken(Guid userId, string email, string role);
}
