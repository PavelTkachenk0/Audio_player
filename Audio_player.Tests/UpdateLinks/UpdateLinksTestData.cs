using Audio_player.DAL;
using Audio_player.DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace Audio_player.Tests.UpdateLinks;

/// <summary>Builds the InMemory database fixtures for the Put-update link-diff tests.</summary>
public static class UpdateLinksTestData
{
    public static AppDbContext NewDb() =>
        new(new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase("links_" + Guid.NewGuid())
            .Options);

    /// <summary>Album #1 linked to artists 10 and 20.</summary>
    public static async Task<AppDbContext> WithAlbumLinkedToArtists10And20()
    {
        var db = NewDb();

        var album = new Album { Id = 1, AlbumName = "X", CoverPath = "c.jpg" };
        album.ArtistsAlbums.Add(new ArtistAlbum { AlbumId = 1, ArtistId = 10 });
        album.ArtistsAlbums.Add(new ArtistAlbum { AlbumId = 1, ArtistId = 20 });
        db.Albums.Add(album);
        await db.SaveChangesAsync();

        return db;
    }
}
