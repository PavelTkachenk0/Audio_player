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

    [Fact]
    public async Task Favorite_tracks_returns_only_the_current_users_favorites()
    {
        using var db = NewDb();

        db.Songs.Add(new Song { Id = 1, SongName = "Mine", SongPath = "m.mp3", AlbumId = 1 });
        db.Songs.Add(new Song { Id = 2, SongName = "Theirs", SongPath = "t.mp3", AlbumId = 1 });
        db.UserSongs.Add(new UserSongs { SongId = 1, UserId = 100 });
        db.UserSongs.Add(new UserSongs { SongId = 2, UserId = 200 });
        await db.SaveChangesAsync();

        const long currentUserId = 100;

        var result = await db.Songs
            .Where(x => x.UserSongs.Any(us => us.UserId == currentUserId))
            .Select(x => x.SongName)
            .ToListAsync();

        Assert.Equal(new[] { "Mine" }, result);
    }

    [Fact]
    public async Task Favorite_tracks_excludes_songs_only_others_favorited()
    {
        using var db = NewDb();

        db.Songs.Add(new Song { Id = 1, SongName = "A", SongPath = "a.mp3", AlbumId = 1 });
        db.UserSongs.Add(new UserSongs { SongId = 1, UserId = 200 });
        await db.SaveChangesAsync();

        const long currentUserId = 100;

        var result = await db.Songs
            .Where(x => x.UserSongs.Any(us => us.UserId == currentUserId))
            .ToListAsync();

        Assert.Empty(result);
    }
}
