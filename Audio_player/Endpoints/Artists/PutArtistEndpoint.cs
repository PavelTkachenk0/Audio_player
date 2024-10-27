using Audio_player.AppSettingsOptions;
using Audio_player.Constants;
using Audio_player.DAL;
using Audio_player.DAL.Models;
using Audio_player.Models.Requests;
using Audio_player.Services;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Audio_player.Endpoints.Artists;

public class PutArtistEndpoint(AppDbContext appDbContext, 
    IOptionsSnapshot<ImageStoreOptions> options, FileService fileService) : Endpoint<PutArtistRequest>
{
    private readonly AppDbContext _appDbContext = appDbContext;
    private readonly ImageStoreOptions _options = options.Value;
    private readonly FileService _fileService = fileService;

    public override void Configure()
    {
        Put("{id:int}");
        Group<ArtistGroup>();
        AllowFileUploads();
        Policies(PolicyNames.HasAdminRole);
    }

    public override async Task HandleAsync(PutArtistRequest req, CancellationToken ct)
    {
        var id = Route<short>("id");

        var artist = await _appDbContext.Artists
            .Include(x => x.GenreArtists)
            .SingleOrDefaultAsync(x => x.Id == id, ct);

        if (artist == null)
        {
            await SendNotFoundAsync(ct);
            return;
        }

        artist.ArtistName = req.ArtistName;

        if (!Directory.Exists(_options.FilesPath))
        {
            Directory.CreateDirectory(_options.FilesPath);
        }

        if (req.Cover != null)
        {
            var coverPath = await _fileService.CreateFile(req.Cover, ct);
            artist.CoverPath = coverPath;
        }

        if (req.Avatar != null)
        {
            var avatarPath = await _fileService.CreateFile(req.Avatar, ct);
            artist.AvatarPath = avatarPath;
        }

        var genresInRequest = req.GenreIds ?? [];

        var genresToDelete = artist.GenreArtists
            .Where(x => !genresInRequest.Contains(x.GenreId) && x.ArtistId == artist.Id);

        var genresToAdd = req.GenreIds?
            .Where(id => !artist.GenreArtists.Any(x => x.GenreId == id))
            .Select(x => new GenreArtist
            {
                GenreId = x,
                ArtistId = artist.Id
            }) ?? [];

        _appDbContext.GenreArtists.RemoveRange(genresToDelete);

        _appDbContext.GenreArtists.AddRange(genresToAdd);

        _appDbContext.Artists.Update(artist);
        await _appDbContext.SaveChangesAsync(ct);

        await SendOkAsync(ct);
    }
}
