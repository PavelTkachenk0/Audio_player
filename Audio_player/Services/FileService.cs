using Audio_player.AppSettingsOptions;
using Audio_player.DAL;
using Microsoft.Extensions.Options;

namespace Audio_player.Services;

public class FileService(AppDbContext appDbContext, IOptionsSnapshot<ImageStoreOptions> imageOptionsSnapshot,
    IOptionsSnapshot<AudioStoreOptions> audioOptionsSnapshot)
{
    private readonly AppDbContext _appDbContext = appDbContext;
    private readonly ImageStoreOptions _imageOptions = imageOptionsSnapshot.Value;
    private readonly AudioStoreOptions _audioOptions = audioOptionsSnapshot.Value;

    public async Task<string> CreateFile(IFormFile file, bool isImage, CancellationToken ct)
    {
        var directory = isImage ? _imageOptions.FilesPath : _audioOptions.FilesPath;

        Directory.CreateDirectory(directory);

        var newFileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName).ToLower();
        var filePath = Path.Combine(directory, newFileName);

        using var fileStream = File.Create(filePath);

        await file.CopyToAsync(fileStream, ct);

        return filePath;
    }

    public void DeleteFile(string filePath)
    {
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }
    }
}
