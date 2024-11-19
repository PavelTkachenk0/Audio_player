using Audio_player.Constants;
using Audio_player.DAL.Models;
using Audio_player.DAL;
using Audio_player.Models.Requests;
using Audio_player.Services;
using FastEndpoints;
using Microsoft.Extensions.Options;
using Audio_player.AppSettingsOptions;
using Microsoft.EntityFrameworkCore;

namespace Audio_player.Endpoints.Tracks;

public class PutTrackEndpoint(AppDbContext appDbContext, 
    IOptionsSnapshot<AudioStoreOptions> optionsSnapshot, FileService fileService) : Endpoint<PutTrackRequest>
{
    private readonly AudioStoreOptions _options = optionsSnapshot.Value;
    private readonly FileService _fileService = fileService;
    private readonly AppDbContext _appDbContext = appDbContext;

    public override void Configure()
    {
        Put("{id:int}");
        Group<TrackGroup>();
        AllowFileUploads();
        Policies(PolicyNames.HasAdminRole);
    }

    public override async Task HandleAsync(PutTrackRequest req, CancellationToken ct)
    {
        var id = Route<long>("id");

        var track = await _appDbContext.Songs
            .Include(x => x.GenreSongs)
            .Include(x => x.ArtistSongs)
            .SingleOrDefaultAsync(x => x.Id == id, ct);

        if (track == null)
        {
            await SendNotFoundAsync(ct);
            return;
        }

        track.SongName = req.SongName;
        track.Duration = req.Duration;
        track.AlbumId = req.AlbumId;

        if (!Directory.Exists(_options.FilesPath))
        {
            Directory.CreateDirectory(_options.FilesPath);
        }

        if (req.SongFile != null)
        {
            var songPath = await _fileService.CreateFile(req.SongFile, false, ct);
            track.SongPath = songPath;
        }

        var genresInRequest = req.GenreIds ?? [];

        var genresToDelete = track.GenreSongs
            .Where(x => !genresInRequest.Contains(x.GenreId) && x.SongId == track.Id);

        var genresToAdd = req.GenreIds?
            .Where(id => !track.GenreSongs.Any(x => x.GenreId == id))
            .Select(x => new GenreSong
            {
                GenreId = x,
                SongId = track.Id
            }) ?? [];

        _appDbContext.GenreSongs.RemoveRange(genresToDelete);

        _appDbContext.GenreSongs.AddRange(genresToAdd);

        var artistsInRequest = req.ArtistIds ?? [];

        var artistsToDelete = track.ArtistSongs
            .Where(x => !artistsInRequest.Contains(x.ArtistId) && x.SongId != track.Id);

        var artistsToAdd = req.ArtistIds?
            .Where(id => !track.ArtistSongs.Any(x => x.ArtistId == id))
            .Select(x => new ArtistSong
            {
                SongId = track.Id,
                ArtistId = x
            }) ?? [];

        _appDbContext.ArtistSongs.RemoveRange(artistsToDelete);

        _appDbContext.ArtistSongs.AddRange(artistsToAdd);

        _appDbContext.Songs.Update(track);
        await _appDbContext.SaveChangesAsync(ct);

        await SendOkAsync(ct);
    }
}
