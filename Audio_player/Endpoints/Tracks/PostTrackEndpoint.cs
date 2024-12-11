using Audio_player.AppSettingsOptions;
using Audio_player.Constants;
using Audio_player.DAL;
using Audio_player.Models.Requests;
using Audio_player.Services;
using FastEndpoints;
using Microsoft.Extensions.Options;

namespace Audio_player.Endpoints.Tracks;

public class PostTrackEndpoint(AppDbContext appDbContext,
    IOptionsSnapshot<AudioStoreOptions> optionsSnapshot, FileService fileService) : Endpoint<PostTrackRequest>
{
    private readonly AppDbContext _appDbContext = appDbContext;
    private readonly AudioStoreOptions _options = optionsSnapshot.Value;
    private readonly FileService _fileService = fileService;

    public override void Configure()
    {
        Post("");
        Group<TrackGroup>();
        AllowFileUploads();
        Policies(PolicyNames.HasAdminRole);
    }

    public override async Task HandleAsync(PostTrackRequest req, CancellationToken ct)
    {
        if (!Directory.Exists(_options.FilesPath))
        {
            Directory.CreateDirectory(_options.FilesPath);
        }

        var songPath = await _fileService.CreateFile(req.SongFile, false, ct);

        var track = _appDbContext.Songs.Add(new DAL.Models.Song
        {
            SongName = req.SongName,
            SongPath = songPath,
            AlbumId = req.AlbumId,
            Duration = req.Duration
        });

        await _appDbContext.SaveChangesAsync(ct);

        _appDbContext.GenreSongs.AddRange(
            req.GenreIds.Select(x => new DAL.Models.GenreSong
            {
                GenreId = x,
                SongId = track.Entity.Id
            })
        );

        _appDbContext.ArtistSongs.AddRange(
            req.ArtistIds.Select(x => new DAL.Models.ArtistSong
            {
                ArtistId = x,
                SongId = track.Entity.Id
            })
        );

        await _appDbContext.SaveChangesAsync(ct);
        await SendOkAsync(ct);
    }
}
