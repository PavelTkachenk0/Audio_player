using Audio_player.AppSettingsOptions;
using Audio_player.DAL;
using Microsoft.Extensions.Options;

namespace Audio_player.Services;

public class FileService(AppDbContext appDbContext, IOptionsSnapshot<ImageStoreOptions> optionsSnapshot)
{
    private readonly AppDbContext _appDbContext = appDbContext;
    private readonly ImageStoreOptions _options = optionsSnapshot.Value;

    public async Task<string> CreateFile(IFormFile file, CancellationToken ct)
    {
        var newFileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName).ToLower();
        var filePath = Path.Combine(_options.FilesPath, newFileName);

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
