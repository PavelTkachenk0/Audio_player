using Audio_player.AppSettingsOptions;
using Audio_player.Constants;
using Audio_player.DAL;
using Audio_player.Models.Requests;
using Audio_player.Services;
using FastEndpoints;
using Microsoft.Extensions.Options;

namespace Audio_player.Endpoints.Genres;

public class PostGenreEndpoint(AppDbContext appDbContext, 
    IOptionsSnapshot<ImageStoreOptions> options, FileService fileService) : Endpoint<CreateGenreRequest>
{
    private readonly ImageStoreOptions _options = options.Value;
    private readonly AppDbContext _appDbContext = appDbContext;
    private readonly FileService _fileService = fileService;

    public override void Configure()
    {
        Post("");
        Group<GenreGroup>();
        AllowFileUploads();
        Policies(PolicyNames.HasAdminRole);
    }

    public async override Task HandleAsync(CreateGenreRequest req, CancellationToken ct)
    {
        if (!Directory.Exists(_options.FilesPath))
        {
            Directory.CreateDirectory(_options.FilesPath);
        }

        var coverPath = await _fileService.CreateFile(req.Cover, ct);

        _appDbContext.Genres.Add(new DAL.Models.Genre
        {
            Name = req.Name,
            CoverPath = coverPath
        });

        await _appDbContext.SaveChangesAsync(ct);
        await SendOkAsync(ct);
    }
}
