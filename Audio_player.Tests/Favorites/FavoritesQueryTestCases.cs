namespace Audio_player.Tests.Favorites;

public static class FavoritesQueryTestCases
{
    /// <summary>(favoriterId, currentUserId, expectedSongName | null when nothing should be returned)</summary>
    public static IEnumerable<object[]> Scoping =>
    [
        [100L, 100L, "Mine"], // a user sees their own favorite
        [200L, 100L, null!] // a user does NOT see someone else's favorite
    ];
}
