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

    // The album starts linked to artists 10 and 20; keepIds is the new request,
    // expectedDeletedIds is what the "to delete" diff should produce.
    [Theory]
    [InlineData(new long[] { 10 }, new long[] { 20 })]        // keep 10 → drop 20
    [InlineData(new long[] { 20 }, new long[] { 10 })]        // keep 20 → drop 10
    [InlineData(new long[] { 10, 20 }, new long[] { })]       // keep both → drop nothing
    [InlineData(new long[] { }, new long[] { 10, 20 })]       // keep none → drop both
    public async Task Put_album_diff_marks_only_links_absent_from_the_request_for_deletion(
        long[] keepIds, long[] expectedDeletedIds)
    {
        using var db = NewDb();

        var album = new Album { Id = 1, AlbumName = "X", CoverPath = "c.jpg" };
        album.ArtistsAlbums.Add(new ArtistAlbum { AlbumId = 1, ArtistId = 10 });
        album.ArtistsAlbums.Add(new ArtistAlbum { AlbumId = 1, ArtistId = 20 });
        db.Albums.Add(album);
        await db.SaveChangesAsync();

        var loaded = await db.Albums
            .Include(x => x.ArtistsAlbums)
            .SingleAsync(x => x.Id == 1);

        var toDelete = loaded.ArtistsAlbums
            .Where(x => !keepIds.Contains(x.ArtistId) && x.AlbumId == loaded.Id)
            .Select(x => x.ArtistId)
            .OrderBy(x => x)
            .ToArray();

        Assert.Equal(expectedDeletedIds, toDelete);
    }
}
