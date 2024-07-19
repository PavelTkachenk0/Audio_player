using Audio_player.DAL;
using Microsoft.AspNetCore.SignalR;

namespace Audio_player.Hubs;

public class AudioHub(AppDbContext appDbContext, IWebHostEnvironment webHostEnvironment) : Hub
{
    private readonly AppDbContext _appDbContext = appDbContext;
    private readonly IWebHostEnvironment _webHostEnvironment = webHostEnvironment;
    
    public async IAsyncEnumerable<byte[]> AudioStream(int fileId)
    {
        var ct = new CancellationToken();

        var file = await _appDbContext.AudioFiles.FindAsync(fileId);
        if (file == null)
        {
            yield break;
        }

        using var stream = new FileStream(file.FilePath, FileMode.Open, FileAccess.Read);
        var buffer = new byte[1024 * 1024];
        int bytesRead;

        while((bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length, ct)) > 0)
        {
            yield return buffer.Take(bytesRead).ToArray();
        }
    }
}
