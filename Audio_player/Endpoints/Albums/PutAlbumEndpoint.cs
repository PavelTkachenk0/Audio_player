using Audio_player.Constants;
using Audio_player.DAL.Models;
using Audio_player.DAL;
using Audio_player.Models.Requests;
using Audio_player.Services;
using FastEndpoints;
using Microsoft.Extensions.Options;
using Audio_player.AppSettingsOptions;
using Microsoft.EntityFrameworkCore;

namespace Audio_player.Endpoints.Albums;

public class PutAlbumEndpoint(AppDbContext appDbContext,
    IOptionsSnapshot<ImageStoreOptions> optionsSnapshot, FileService fileService) : Endpoint<PutAlbumRequest>
{
    private readonly AppDbContext _appDbContext = appDbContext;
    private readonly ImageStoreOptions _options = optionsSnapshot.Value;
    private readonly FileService _fileService = fileService;

    public override void Configure()
    {
        Put("{id:int}");
        Group<AlbumGroup>();
        AllowFileUploads();
        Policies(PolicyNames.HasAdminRole);
    }

    public override async Task HandleAsync(PutAlbumRequest req, CancellationToken ct)
    {
        var id = Route<long>("id");

        var album = await _appDbContext.Albums
            .Include(x => x.GenreAlbums)
            .Include(x => x.ArtistsAlbums)
            .SingleOrDefaultAsync(x => x.Id == id, ct);

        if (album == null)
        {
            await SendNotFoundAsync(ct);
            return;
        }

        album.AlbumName = req.AlbumName;

        if (!Directory.Exists(_options.FilesPath))
        {
            Directory.CreateDirectory(_options.FilesPath);
        }

        if (req.Cover != null)
        {
            var coverPath = await _fileService.CreateFile(req.Cover, true, ct);
            album.CoverPath = coverPath;
        }

        var genresInRequest = req.GenreIds ?? [];

        var genresToDelete = album.GenreAlbums
            .Where(x => !genresInRequest.Contains(x.GenreId) && x.AlbumId == album.Id);

        var genresToAdd = req.GenreIds?
            .Where(id => !album.GenreAlbums.Any(x => x.GenreId == id))
            .Select(x => new GenreAlbum
            {
                GenreId = x,
                AlbumId = album.Id
            }) ?? [];

        _appDbContext.GenreAlbums.RemoveRange(genresToDelete);

        _appDbContext.GenreAlbums.AddRange(genresToAdd);

        var artistsInRequest = req.ArtistIds ?? [];

        var artistsToDelete = album.ArtistsAlbums
            .Where(x => !artistsInRequest.Contains(x.ArtistId) && x.AlbumId != album.Id);

        var artistsToAdd = req.ArtistIds?
            .Where(id => !album.ArtistsAlbums.Any(x => x.ArtistId == id))
            .Select(x => new ArtistAlbum
            {
                AlbumId = album.Id,
                ArtistId = x
            }) ?? [];

        _appDbContext.ArtistAlbums.RemoveRange(artistsToDelete);

        _appDbContext.ArtistAlbums.AddRange(artistsToAdd);

        _appDbContext.Albums.Update(album);
        await _appDbContext.SaveChangesAsync(ct);

        await SendOkAsync(ct);
    }
}
