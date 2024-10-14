using Audio_player.DAL;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace Audio_player.Hubs;

public class AudioHub(AppDbContext appDbContext, IWebHostEnvironment webHostEnvironment) : Hub
{
    private readonly AppDbContext _appDbContext = appDbContext;
    private readonly IWebHostEnvironment _webHostEnvironment = webHostEnvironment;
    
    public async IAsyncEnumerable<byte[]> AudioStream(int fileId)
    {
        var ct = new CancellationToken();

        var file = await _appDbContext.AudioFiles.Where(x => x.Id == fileId).SingleOrDefaultAsync(ct);
        if (file == null)
        {
            yield break;
        }

        using var stream = new FileStream(file.FilePath, FileMode.Open, FileAccess.Read);
        var buffer = new byte[1024 * 1024];
        int bytesRead;
        var offset = 0;

        while ((bytesRead = await stream.ReadAsync(buffer.AsMemory(offset, buffer.Length), ct)) > 0)
        {
            yield return buffer.Take(bytesRead).ToArray();
        }
    }
}
