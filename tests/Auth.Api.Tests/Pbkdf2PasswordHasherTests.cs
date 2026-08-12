using Auth.Api.Infrastructure.Security;

namespace Auth.Api.Tests;

public sealed class Pbkdf2PasswordHasherTests
{
    private const string Password = "P@ssw0rd123";
    private const string LegacySha256Hash = "231ECC7D178DA5F22983BC579599396D6C139A457987AE1EE0026D88432D6A72";

    private readonly Pbkdf2PasswordHasher hasher = new();

    [Fact]
    public void Hash_ShouldGeneratePbkdf2FormattedHash()
    {
        var hash = hasher.Hash(Password);

        Assert.StartsWith("pbkdf2-sha256.v1$", hash, StringComparison.Ordinal);
        Assert.True(hasher.Verify(Password, hash));
    }

    [Fact]
    public void Verify_ShouldSupportLegacySha256Hashes()
    {
        var verified = hasher.Verify(Password, LegacySha256Hash);

        Assert.True(verified);
    }

    [Fact]
    public void Verify_ShouldReturnFalse_ForInvalidPassword()
    {
        var hash = hasher.Hash(Password);

        var verified = hasher.Verify("wrong-password", hash);

        Assert.False(verified);
    }
}
