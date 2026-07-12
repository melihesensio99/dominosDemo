using Auth.Api.Abstractions;

namespace Auth.Api.Features.Register;

public sealed class RegisterHandler(
    IUserRepository userRepository,
    IPasswordHasher passwordHasher,
    ITokenService tokenService) : IRequestHandler<RegisterCommand, Result<RegisterResponse>>
{
    public async Task<Result<RegisterResponse>> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var existingUser = await userRepository.GetByEmailAsync(request.Email, cancellationToken);
        if (existingUser is not null)
        {
            return Result<RegisterResponse>.Conflict("auth.email_exists", "A user with this email already exists.");
        }

        var user = new User
        {
            Email = request.Email.Trim().ToLowerInvariant(),
            PasswordHash = passwordHasher.Hash(request.Password),
            Role = "User",
        };

        await userRepository.AddAsync(user, cancellationToken);

        var token = tokenService.CreateToken(user.Id, user.Email, user.Role);
        return Result<RegisterResponse>.Success(new RegisterResponse(user.Id, user.Email, user.Role, token));
    }
}
