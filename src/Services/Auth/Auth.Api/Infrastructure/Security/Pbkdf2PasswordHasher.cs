using System.Security.Cryptography;
using System.Text;

using Auth.Api.Application.Abstractions.Security;

namespace Auth.Api.Infrastructure.Security;

public sealed class Pbkdf2PasswordHasher : IPasswordHasher
{
    private const string Format = "pbkdf2-sha256.v1";
    private const int SaltSize = 16;
    private const int HashSize = 32;
    private const int Iterations = 120_000;

    public string Hash(string password)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(password);

        var salt = RandomNumberGenerator.GetBytes(SaltSize);
        var derivedKey = Rfc2898DeriveBytes.Pbkdf2(
            Encoding.UTF8.GetBytes(password),
            salt,
            Iterations,
            HashAlgorithmName.SHA256,
            HashSize);

        return string.Join('$', Format, Iterations, Convert.ToBase64String(salt), Convert.ToBase64String(derivedKey));
    }

    public bool Verify(string password, string hash)
    {
        if (string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(hash))
        {
            return false;
        }

        return hash.StartsWith($"{Format}$", StringComparison.Ordinal)
            ? VerifyPbkdf2(password, hash)
            : VerifyLegacySha256(password, hash);
    }

    private static bool VerifyPbkdf2(string password, string encodedHash)
    {
        var parts = encodedHash.Split('$');
        if (parts.Length != 4
            || !int.TryParse(parts[1], out var iterations)
            || iterations <= 0)
        {
            return false;
        }

        try
        {
            var salt = Convert.FromBase64String(parts[2]);
            var expectedHash = Convert.FromBase64String(parts[3]);
            var actualHash = Rfc2898DeriveBytes.Pbkdf2(
                Encoding.UTF8.GetBytes(password),
                salt,
                iterations,
                HashAlgorithmName.SHA256,
                expectedHash.Length);

            return CryptographicOperations.FixedTimeEquals(actualHash, expectedHash);
        }
        catch (FormatException)
        {
            return false;
        }
    }

    private static bool VerifyLegacySha256(string password, string hash)
    {
        try
        {
            var legacyHash = SHA256.HashData(Encoding.UTF8.GetBytes(password));
            var expectedHash = Convert.FromHexString(hash);
            return CryptographicOperations.FixedTimeEquals(legacyHash, expectedHash);
        }
        catch (FormatException)
        {
            return false;
        }
    }
}
