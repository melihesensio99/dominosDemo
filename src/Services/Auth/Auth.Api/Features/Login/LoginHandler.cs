using Auth.Api.Abstractions;

namespace Auth.Api.Features.Login;

public sealed class LoginHandler(
    IUserRepository userRepository,
    IPasswordHasher passwordHasher,
    ITokenService tokenService) : IRequestHandler<LoginCommand, Result<LoginResponse>>
{
    public async Task<Result<LoginResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByEmailAsync(request.Email, cancellationToken);
        if (user is null)
        {
            return Result<LoginResponse>.Unauthorized("auth.invalid_credentials", "Invalid email or password.");
        }

        if (!passwordHasher.Verify(request.Password, user.PasswordHash))
        {
            return Result<LoginResponse>.Unauthorized("auth.invalid_credentials", "Invalid email or password.");
        }

        var token = tokenService.CreateToken(user.Id, user.Email, user.Role);
        return Result<LoginResponse>.Success(new LoginResponse(user.Id, user.Email, user.Role, token));
    }
}
