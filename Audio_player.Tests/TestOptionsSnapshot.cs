using Microsoft.Extensions.Options;

namespace Audio_player.Tests;

/// <summary>Minimal IOptionsSnapshot wrapper for exercising options-consuming services in tests.</summary>
public class TestOptionsSnapshot<T>(T value) : IOptionsSnapshot<T> where T : class
{
    public T Value => value;

    public T Get(string? name) => value;
}
