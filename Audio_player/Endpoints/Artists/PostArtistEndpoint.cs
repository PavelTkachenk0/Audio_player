using Audio_player.AppSettingsOptions;
using Audio_player.Constants;
using Audio_player.DAL;
using Audio_player.Models.Requests;
using Audio_player.Services;
using FastEndpoints;
using Microsoft.Extensions.Options;

namespace Audio_player.Endpoints.Artists;

public class PostArtistEndpoint(IOptionsSnapshot<ImageStoreOptions> optionsSnapshot, 
    AppDbContext appDbContext, FileService fileService) : Endpoint<PostArtistRequest>
{
    private readonly ImageStoreOptions _options = optionsSnapshot.Value;
    private readonly AppDbContext _appDbContext = appDbContext;
    private readonly FileService _fileService = fileService;

    public override void Configure()
    {
        Post("");
        Group<ArtistGroup>();
        AllowFileUploads();
        Policies(PolicyNames.HasAdminRole);
    }

    public override async Task HandleAsync(PostArtistRequest req, CancellationToken ct)
    {
        if (!Directory.Exists(_options.FilesPath))
        {
            Directory.CreateDirectory(_options.FilesPath);
        }

        var coverPath = await _fileService.CreateFile(req.Cover, ct);
        var avatarPath = await _fileService.CreateFile(req.Avatar, ct);

        var artist = _appDbContext.Artists.Add(new DAL.Models.Artist
        {
            ArtistName = req.ArtistName,
            CoverPath = coverPath,
            AvatarPath = avatarPath
        });

        await _appDbContext.SaveChangesAsync(ct);

        foreach (var genreId in req.GenreIds)
        {
            _appDbContext.GenreArtists.Add(new DAL.Models.GenreArtist
            {
                ArtistId = artist.Entity.Id,
                GenreId = genreId,
            });
        }

        await _appDbContext.SaveChangesAsync(ct);
        await SendOkAsync(ct);
    }
}
