using Audio_player.AppSettingsOptions;
using Audio_player.Constants;
using Audio_player.DAL;
using Audio_player.Models.Requests;
using Audio_player.Services;
using FastEndpoints;
using Microsoft.Extensions.Options;

namespace Audio_player.Endpoints.Albums;

public class PostAlbumEndpoint(AppDbContext appDbContext, 
    IOptionsSnapshot<ImageStoreOptions> optionsSnapshot, FileService fileService) : Endpoint<PostAlbumRequest>
{
    private readonly AppDbContext _appDbContext = appDbContext;
    private readonly FileService _fileService = fileService;
    private readonly ImageStoreOptions _options = optionsSnapshot.Value;

    public override void Configure()
    {
        Post("");
        Group<AlbumGroup>();
        AllowFileUploads();
        Policies(PolicyNames.HasAdminRole);
    }

    public override async Task HandleAsync(PostAlbumRequest req, CancellationToken ct)
    {
        if (!Directory.Exists(_options.FilesPath))
        {
            Directory.CreateDirectory(_options.FilesPath);
        }

        var coverPath = await _fileService.CreateFile(req.Cover, ct);

        var album = _appDbContext.Albums.Add(new DAL.Models.Album
        {
            AlbumName = req.AlbumName,
            CoverPath = coverPath
        });

        await _appDbContext.SaveChangesAsync(ct);

        _appDbContext.GenreAlbums.AddRange(
            req.GenreIds.Select(x => new DAL.Models.GenreAlbum
            {
                GenreId = x,
                AlbumId = album.Entity.Id
            })
        );

        _appDbContext.ArtistAlbums.AddRange(
            req.ArtistIds.Select(x => new DAL.Models.ArtistAlbum
            {
                ArtistId = x,
                AlbumId = album.Entity.Id
            })
        );

        await _appDbContext.SaveChangesAsync(ct);
        await SendOkAsync(ct);
    }
}
