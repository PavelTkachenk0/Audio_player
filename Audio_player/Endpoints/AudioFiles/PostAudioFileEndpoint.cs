using Audio_player.AppSettingsOptions;
using Audio_player.DAL;
using Audio_player.Endpoints.Files;
using Audio_player.Models.Requests;
using FastEndpoints;
using Microsoft.Extensions.Options;

namespace Audio_player.Endpoints.AudioFiles;

public class PostAudioFileEndpoint(IOptionsSnapshot<FileStoreOptions> optionsSnapshot, AppDbContext appDbContext) : Endpoint<PostAudioFileRequest>
{
    private readonly FileStoreOptions _options = optionsSnapshot.Value;
    private readonly AppDbContext _appDbContext = appDbContext;

    public override void Configure()
    {
        Post("");
        Group<AudioFileGroup>();
        AllowAnonymous();
        AllowFileUploads(dontAutoBindFormData: true);
    }

    public override async Task HandleAsync(PostAudioFileRequest req, CancellationToken ct)
    {
        if (!Directory.Exists(_options.FilesPath))
        {
            Directory.CreateDirectory(_options.FilesPath);
        }

        foreach(var file in Files)
        {
            var fileName = file.FileName;
            var filePath = Path.Combine(_options.FilesPath, fileName);

            using var fileStream = File.Create(filePath);

            await file.CopyToAsync(fileStream, ct);

            _appDbContext.AudioFiles.Add(new DAL.Models.AudioFile
            {
                FileName = fileName,
                FilePath = filePath
            });
        }

        await _appDbContext.SaveChangesAsync(ct);
        await SendOkAsync(ct);
    }
}
