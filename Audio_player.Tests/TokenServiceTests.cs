using Audio_player.AppSettingsOptions;
using Audio_player.DAL;
using Audio_player.Helpers;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Audio_player.Tests;

public class TokenServiceTests
{
    private static GenerateTokenService CreateService(string key = "test_signing_key_at_least_32_bytes_long_0123456789")
    {
        var db = new AppDbContext(new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase("tok_" + Guid.NewGuid())
            .Options);

        var authOptions = new AuthOptions { Issuer = "test_iss", Audience = "test_aud", Key = key };

        return new GenerateTokenService(new TestOptionsSnapshot<AuthOptions>(authOptions), db);
    }

    [Fact]
    public void TwoFactorPendingToken_roundtrips_back_to_the_email()
    {
        var svc = CreateService();

        var token = svc.GenerateTwoFactorPendingToken("user@test.com");

        Assert.Equal("user@test.com", svc.ValidateTwoFactorPendingToken(token));
    }

    [Theory]
    [InlineData("")]
    [InlineData("garbage")]
    [InlineData("not.a.valid.token")]
    [InlineData("a.b.c")]
    public void ValidateTwoFactorPendingToken_returns_null_for_malformed_input(string token)
    {
        var svc = CreateService();

        Assert.Null(svc.ValidateTwoFactorPendingToken(token));
    }

    [Fact]
    public void ValidateTwoFactorPendingToken_rejects_a_token_signed_with_a_different_key()
    {
        var issuer = CreateService(key: "the_original_signing_key_padded_to_32_bytes_xxx");
        var token = issuer.GenerateTwoFactorPendingToken("user@test.com");

        var attacker = CreateService(key: "a_completely_different_signing_key_padded_xxxxx");

        Assert.Null(attacker.ValidateTwoFactorPendingToken(token));
    }
}
