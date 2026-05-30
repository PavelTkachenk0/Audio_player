using Audio_player.Services;
using Xunit;

namespace Audio_player.Tests;

public class PasswordHasherTests
{
    private readonly IPasswordHasher _hasher = new BcryptPasswordHasher();

    [Fact]
    public void Hash_does_not_return_the_plaintext()
    {
        var hash = _hasher.Hash("correct horse battery staple");

        Assert.NotEqual("correct horse battery staple", hash);
        Assert.StartsWith("$2", hash); // bcrypt hashes start with $2a/$2b/$2y
    }

    [Theory]
    [InlineData("p@ssw0rd123", "p@ssw0rd123", true)]    // correct password verifies
    [InlineData("p@ssw0rd123", "totally-wrong", false)] // wrong password rejected
    [InlineData("p@ssw0rd123", "P@ssw0rd123", false)]   // case-sensitive
    [InlineData("p@ssw0rd123", "", false)]              // empty attempt rejected
    public void Verify_matches_only_the_exact_original_password(string original, string attempt, bool expected)
    {
        var hash = _hasher.Hash(original);

        Assert.Equal(expected, _hasher.Verify(attempt, hash));
    }

    [Fact]
    public void Verify_returns_false_for_a_legacy_plaintext_value_without_throwing()
    {
        // Rows created before hashing stored the password verbatim. Verify must treat
        // such a value as a mismatch instead of crashing on an invalid bcrypt salt.
        Assert.False(_hasher.Verify("root", "root"));
    }
}
