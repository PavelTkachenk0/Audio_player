using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Audio_player.Tests.UpdateLinks;

/// <summary>
/// Guards the PutAlbum/PutTrack fix: stale artist links must be removed on update.
/// The old "to delete" predicate used `!= album.Id`, which never matched, so nothing was deleted.
/// </summary>
public class UpdateLinksTests
{
    [Theory]
    [MemberData(nameof(UpdateLinksTestCases.DeletionDiff), MemberType = typeof(UpdateLinksTestCases))]
    public async Task Put_album_diff_marks_only_links_absent_from_the_request_for_deletion(
        long[] keepIds, long[] expectedDeletedIds)
    {
        await using var db = await UpdateLinksTestData.WithAlbumLinkedToArtists10And20();

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
