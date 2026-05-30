using Audio_player.DAL;
using Audio_player.DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace Audio_player.Tests.Favorites;

/// <summary>Builds the InMemory database fixtures for the favorites-scoping tests.</summary>
public static class FavoritesQueryTestData
{
    private static AppDbContext NewDb() =>
        new(new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase("fav_" + Guid.NewGuid())
            .Options);

    /// <summary>A single song "Mine" favorited by <paramref name="favoriterId"/>.</summary>
    public static async Task<AppDbContext> WithFavoritedSong(long favoriterId)
    {
        var db = NewDb();

        db.Songs.Add(new Song { Id = 1, SongName = "Mine", SongPath = "m.mp3", AlbumId = 1 });
        db.UserSongs.Add(new UserSongs { SongId = 1, UserId = favoriterId });
        await db.SaveChangesAsync();

        return db;
    }
}
