using Audio_player.AppSettingsOptions;
using Audio_player.Constants;
using Audio_player.DAL;
using Audio_player.Models.Requests;
using FastEndpoints;
using Microsoft.Extensions.Options;

namespace Audio_player.Endpoints.Genres;

public class PostGenreEndpoint(AppDbContext appDbContext, IOptionsSnapshot<ImageStoreOptions> options) : Endpoint<CreateGenreRequest>
{
    private readonly ImageStoreOptions _options = options.Value;
    private readonly AppDbContext _appDbContext = appDbContext;

    public override void Configure()
    {
        Post("");
        Group<GenreGroup>();
        AllowFileUploads();
        Policies(PolicyNames.HasAdminRole);
    }

    public async override Task HandleAsync(CreateGenreRequest req, CancellationToken ct)
    {
        if (!Files.Any())
        {
            ThrowError("File can not be empty");
        }

        if (!Directory.Exists(_options.FilesPath))
        {
            Directory.CreateDirectory(_options.FilesPath);
        }

        foreach (var file in Files)
        {
            var newFileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName).ToLower();
            var filePath = Path.Combine(_options.FilesPath, newFileName);

            using var fileStream = File.Create(filePath);

            await file.CopyToAsync(fileStream, ct);

            _appDbContext.Genres.Add(new DAL.Models.Genre
            {
                Name = req.Name,
                CoverPath = filePath
            });
        }

        await _appDbContext.SaveChangesAsync(ct);
        await SendOkAsync(ct);
    }
}
