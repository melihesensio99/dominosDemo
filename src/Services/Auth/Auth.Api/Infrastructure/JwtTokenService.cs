using System.Security.Claims;
using System.Text;
using Auth.Api.Abstractions.Application;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace Auth.Api.Infrastructure;

public sealed class JwtTokenService(IConfiguration configuration) : ITokenService
{
    private readonly string _issuer = configuration["Jwt:Issuer"]
        ?? throw new InvalidOperationException("Jwt:Issuer is missing.");

    private readonly string _audience = configuration["Jwt:Audience"]
        ?? throw new InvalidOperationException("Jwt:Audience is missing.");

    private readonly int _accessTokenMinutes = int.TryParse(configuration["Jwt:AccessTokenMinutes"], out var minutes)
        ? minutes
        : 60;

    private readonly SigningCredentials _signingCredentials = CreateSigningCredentials(configuration);

    public string CreateToken(Guid userId, string email, string role)
    {
        var issuedAt = DateTime.UtcNow;
        var claims = new ClaimsIdentity([
            new Claim("sub", userId.ToString()),
            new Claim("email", email),
            new Claim(ClaimTypes.Role, role),
            new Claim("jti", Guid.NewGuid().ToString("N")),
        ]);

        var descriptor = new SecurityTokenDescriptor
        {
            Issuer = _issuer,
            Audience = _audience,
            Subject = claims,
            NotBefore = issuedAt,
            IssuedAt = issuedAt,
            Expires = issuedAt.AddMinutes(_accessTokenMinutes),
            SigningCredentials = _signingCredentials,
        };

        return new JsonWebTokenHandler().CreateToken(descriptor);
    }

    private static SigningCredentials CreateSigningCredentials(IConfiguration configuration)
    {
        var signingKey = configuration["Jwt:SigningKey"]
            ?? throw new InvalidOperationException("Jwt:SigningKey is missing.");

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey));
        return new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
    }
}
