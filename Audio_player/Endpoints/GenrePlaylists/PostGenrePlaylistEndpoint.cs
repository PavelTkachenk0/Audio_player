using Audio_player.AppSettingsOptions;
using Audio_player.Constants;
using Audio_player.DAL;
using Audio_player.Models.Requests;
using Audio_player.Services;
using FastEndpoints;
using Microsoft.Extensions.Options;

namespace Audio_player.Endpoints.GenrePlaylists;

public class PostGenrePlaylistEndpoint(AppDbContext appDbContext,
    IOptionsSnapshot<ImageStoreOptions> optionsSnapshot, FileService fileService) : Endpoint<PostPlaylistRequest>
{
    private readonly AppDbContext _appDbContext = appDbContext;
    private readonly FileService _fileService = fileService;
    private readonly ImageStoreOptions _options = optionsSnapshot.Value;

    public override void Configure()
    {
        Post("");
        Group<GenrePlaylistGroup>();
        AllowFileUploads();
        Policies(PolicyNames.HasAdminRole);
    }

    public override async Task HandleAsync(PostPlaylistRequest req, CancellationToken ct)
    {
        if (!Directory.Exists(_options.FilesPath))
        {
            Directory.CreateDirectory(_options.FilesPath);
        }

        var coverPath = await _fileService.CreateFile(req.Cover, true, ct);

        var playlist = _appDbContext.Playlists.Add(new DAL.Models.Playlist
        {
            Name = req.Name,
            CoverPath = coverPath,
            IsAdmin = true
        });

        await _appDbContext.SaveChangesAsync(ct);

        _appDbContext.PlaylistSongs.AddRange(
            req.TrackIds.Select(x => new DAL.Models.PlaylistSong
            {
                SongId = x,
                PlaylistId = playlist.Entity.Id
            })
        );

        await _appDbContext.SaveChangesAsync(ct);
        await SendOkAsync(ct);
    }
}
