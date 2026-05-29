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

    [Fact]
    public void Verify_returns_true_for_the_correct_password()
    {
        var hash = _hasher.Hash("p@ssw0rd123");

        Assert.True(_hasher.Verify("p@ssw0rd123", hash));
    }

    [Fact]
    public void Verify_returns_false_for_a_wrong_password()
    {
        var hash = _hasher.Hash("p@ssw0rd123");

        Assert.False(_hasher.Verify("totally-wrong", hash));
    }

    [Fact]
    public void Verify_returns_false_for_a_legacy_plaintext_value_without_throwing()
    {
        // Rows created before hashing stored the password verbatim. Verify must treat
        // such a value as a mismatch instead of crashing on an invalid bcrypt salt.
        Assert.False(_hasher.Verify("root", "root"));
    }
}
