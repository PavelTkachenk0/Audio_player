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

        var song = await _appDbContext.Songs.Where(x => x.Id == fileId).SingleOrDefaultAsync(ct);
        if (song == null)
        {
            yield break;
        }

        using var stream = new FileStream(song.SongPath, FileMode.Open, FileAccess.Read);
        var buffer = new byte[1024 * 1024];
        int bytesRead;
        var offset = 0;

        try
        {
            while ((bytesRead = await stream.ReadAsync(buffer.AsMemory(offset, buffer.Length), ct)) > 0)
            {
                yield return buffer.Take(bytesRead).ToArray();
            }
        }
        finally
        {
            song.ListeningCount++;

            _appDbContext.Songs.Update(song);
            await _appDbContext.SaveChangesAsync(ct);
        }
    }
}
