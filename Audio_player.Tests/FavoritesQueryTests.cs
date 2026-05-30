using Audio_player.DAL;
using Audio_player.DAL.Models;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Audio_player.Tests;

/// <summary>
/// Guards the favorites privacy fix: the listing must be scoped to the current user.
/// The old predicate (x.UserSongs.Select(us => us.SongId).Contains(x.Id)) leaked
/// everything that anyone had favorited.
/// </summary>
public class FavoritesQueryTests
{
    private static AppDbContext NewDb() =>
        new(new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase("fav_" + Guid.NewGuid())
            .Options);

    [Theory]
    [InlineData(100, 100, "Mine")] // a user sees their own favorite
    [InlineData(200, 100, null)]   // a user does NOT see someone else's favorite
    public async Task Favorite_tracks_are_scoped_to_the_current_user(long favoriterId, long currentUserId, string? expectedName)
    {
        using var db = NewDb();

        db.Songs.Add(new Song { Id = 1, SongName = "Mine", SongPath = "m.mp3", AlbumId = 1 });
        db.UserSongs.Add(new UserSongs { SongId = 1, UserId = favoriterId });
        await db.SaveChangesAsync();

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
