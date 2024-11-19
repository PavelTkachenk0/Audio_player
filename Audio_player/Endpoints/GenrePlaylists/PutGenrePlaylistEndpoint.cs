using Audio_player.AppSettingsOptions;
using Audio_player.Constants;
using Audio_player.DAL;
using Audio_player.DAL.Models;
using Audio_player.Models.Requests;
using Audio_player.Services;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Audio_player.Endpoints.GenrePlaylists;

public class PutGenrePlaylistEndpoint(AppDbContext appDbContext,
    IOptionsSnapshot<ImageStoreOptions> optionsSnapshot, FileService fileService) : Endpoint<PutPlaylistRequest>
{
    private readonly AppDbContext _appDbContext = appDbContext;
    private readonly ImageStoreOptions _options = optionsSnapshot.Value;
    private readonly FileService _fileService = fileService;

    public override void Configure()
    {
        Put("{id:int}");
        Group<GenrePlaylistGroup>();
        AllowFileUploads();
        Policies(PolicyNames.HasAdminRole);
    }

    public override async Task HandleAsync(PutPlaylistRequest req, CancellationToken ct)
    {
        var id = Route<long>("id");

        var playlist = await _appDbContext.Playlists
            .Include(x => x.PlaylistSongs)
            .SingleOrDefaultAsync(x => x.Id == id, ct);

        if (playlist == null)
        {
            await SendNotFoundAsync(ct);
            return;
        }

        playlist.Name = req.Name;

        if (!Directory.Exists(_options.FilesPath))
        {
            Directory.CreateDirectory(_options.FilesPath);
        }

        if (req.Cover != null)
        {
            var coverPath = await _fileService.CreateFile(req.Cover, true, ct);
            playlist.CoverPath = coverPath;
        }

        var songsInRequest = req.TrackIds ?? [];

        var songsToDelete = playlist.PlaylistSongs
            .Where(x => !songsInRequest.Contains(x.SongId) && x.PlaylistId == playlist.Id);

        var songsToAdd = req.TrackIds?
            .Where(id => !playlist.PlaylistSongs.Any(x => x.SongId == id))
            .Select(x => new PlaylistSong
            {
                SongId = x,
                PlaylistId = playlist.Id
            }) ?? [];

        _appDbContext.PlaylistSongs.RemoveRange(songsToDelete);

        _appDbContext.PlaylistSongs.AddRange(songsToAdd);

        _appDbContext.Playlists.Update(playlist);
        await _appDbContext.SaveChangesAsync(ct);

        await SendOkAsync(ct);
    }
}
