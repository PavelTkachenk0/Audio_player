using Audio_player.AppSettingsOptions;
using Audio_player.Constants;
using Audio_player.DAL;
using Audio_player.Models.Requests;
using Audio_player.Services;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Audio_player.Endpoints.Genres;

public class PutGenreEndpoint(AppDbContext appDbContext, 
    IOptionsSnapshot<ImageStoreOptions> optionsSnapshot, FileService fileService) : Endpoint<EditGenreRequest>
{
    private readonly AppDbContext _appDbContext = appDbContext;
    private readonly ImageStoreOptions _options = optionsSnapshot.Value;
    private readonly FileService _fileService = fileService;

    public override void Configure()
    {
        Put("{id:int}");
        Group<GenreGroup>();
        AllowFileUploads();
        Policies(PolicyNames.HasAdminRole);
    }

    public override async Task HandleAsync(EditGenreRequest req, CancellationToken ct)
    {
        var id = Route<short>("id");

        var genre = await _appDbContext.Genres.SingleOrDefaultAsync(x => x.Id == id, ct);

        if (genre == null)
        {
            await SendNotFoundAsync(ct);
            return;
        }

        genre.Name = req.Name;

        if (!Directory.Exists(_options.FilesPath))
        {
            Directory.CreateDirectory(_options.FilesPath);
        }

        if (req.Cover != null)
        {
            var coverPath = await _fileService.CreateFile(req.Cover, true, ct);

            genre.CoverPath = coverPath;
        }

        _appDbContext.Genres.Update(genre);
        await _appDbContext.SaveChangesAsync(ct);

        await SendOkAsync(ct);
    }
}
