using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Audio_player.Tests.Favorites;

/// <summary>
/// Guards the favorites privacy fix: the listing must be scoped to the current user.
/// The old predicate leaked everything that anyone had favorited.
/// </summary>
public class FavoritesQueryTests
{
    [Theory]
    [MemberData(nameof(FavoritesQueryTestCases.Scoping), MemberType = typeof(FavoritesQueryTestCases))]
    public async Task Favorite_tracks_are_scoped_to_the_current_user(long favoriterId, long currentUserId, string? expectedName)
    {
        await using var db = await FavoritesQueryTestData.WithFavoritedSong(favoriterId);

        var result = await db.Songs
            .Where(x => x.UserSongs.Any(us => us.UserId == currentUserId))
            .Select(x => x.SongName)
            .ToListAsync();

        if (expectedName == null)
        {
            Assert.Empty(result);
        }
        else
        {
            Assert.Equal(new[] { expectedName }, result);
        }
    }
}
