using Audio_player.Services;
using Xunit;

namespace Audio_player.Tests;

public class PasswordHasherTests
{
    private readonly IPasswordHasher _hasher = new BcryptPasswordHasher();

    [Theory]
    [InlineData("correct horse battery staple")]
    [InlineData("p@ssw0rd123")]
    [InlineData("короткий")]
    public void Hash_produces_a_bcrypt_hash_distinct_from_the_input(string password)
    {
        var hash = _hasher.Hash(password);

        Assert.NotEqual(password, hash);
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

    [Theory]
    [InlineData("root")]
    [InlineData("password")]
    [InlineData("123456")]
    public void Verify_returns_false_for_a_legacy_plaintext_value_without_throwing(string legacyPlaintext)
    {
        // Rows created before hashing stored the password verbatim. Verify must treat such a
        // value as a mismatch instead of crashing on an invalid bcrypt salt.
        Assert.False(_hasher.Verify(legacyPlaintext, legacyPlaintext));
    }
}
