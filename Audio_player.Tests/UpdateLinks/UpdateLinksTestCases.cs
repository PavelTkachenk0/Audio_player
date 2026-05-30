namespace Audio_player.Tests.UpdateLinks;

public static class UpdateLinksTestCases
{
    /// <summary>(artistIds kept in the request, artistIds expected in the "to delete" diff)</summary>
    public static IEnumerable<object[]> DeletionDiff => new[]
    {
        new object[] { new long[] { 10 }, new long[] { 20 } },          // keep 10 → drop 20
        new object[] { new long[] { 20 }, new long[] { 10 } },          // keep 20 → drop 10
        new object[] { new long[] { 10, 20 }, Array.Empty<long>() },    // keep both → drop nothing
        new object[] { Array.Empty<long>(), new long[] { 10, 20 } },    // keep none → drop both
    };
}
