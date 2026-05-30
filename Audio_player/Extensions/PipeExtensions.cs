namespace Audio_player.Extensions;

/// <summary>
/// Functional pipe-forward helpers (cf. F# <c>|></c>, Kotlin <c>let</c>/<c>also</c>).
/// Most useful for composing an <see cref="IQueryable{T}"/> in a single fluent chain —
/// e.g. conditional steps — without breaking it into intermediate variables.
/// Note: these run while BUILDING the query (in C#), so they compose fine; do NOT call
/// them inside a translated lambda (Select/Where body) — EF cannot translate them to SQL.
/// </summary>
public static class PipeExtensions
{
    /// <summary>Maps the value into another (or the same) type: <c>value.PipeAs(x => f(x))</c>.</summary>
    public static TResult PipeAs<T, TResult>(this T value, Func<T, TResult> map) => map(value);

    /// <summary>Runs a side effect on the value and returns it unchanged (tee).</summary>
    public static T Pipe<T>(this T value, Action<T> action)
    {
        action(value);
        return value;
    }
}
