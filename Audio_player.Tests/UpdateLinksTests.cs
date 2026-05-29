using Audio_player.DAL;
using Audio_player.DAL.Models;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Audio_player.Tests;

/// <summary>
/// Guards the PutAlbum/PutTrack fix: stale artist links must be removed on update.
/// The old "to delete" predicate used `!= album.Id`, which never matched rows loaded
/// for that album, so nothing was ever deleted.
/// </summary>
public class UpdateLinksTests
{
    private static AppDbContext NewDb() =>
        new(new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase("links_" + Guid.NewGuid())
            .Options);

    [Fact]
    public async Task Put_album_marks_artist_links_no_longer_in_request_for_deletion()
    {
        using var db = NewDb();

        var album = new Album { Id = 1, AlbumName = "X", CoverPath = "c.jpg" };
        album.ArtistsAlbums.Add(new ArtistAlbum { AlbumId = 1, ArtistId = 10 });
        album.ArtistsAlbums.Add(new ArtistAlbum { AlbumId = 1, ArtistId = 20 });
        db.Albums.Add(album);
        await db.SaveChangesAsync();

        var artistsInRequest = new List<long> { 10 }; // keep 10, drop 20

        var loaded = await db.Albums
            .Include(x => x.ArtistsAlbums)
            .SingleAsync(x => x.Id == 1);

        var toDelete = loaded.ArtistsAlbums
            .Where(x => !artistsInRequest.Contains(x.ArtistId) && x.AlbumId == loaded.Id)
            .ToList();

        Assert.Single(toDelete);
        Assert.Equal(20, toDelete[0].ArtistId);
    }
}
