using Audio_player.DAL;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace Audio_player.Hubs;

[Authorize]
public class AudioHub(AppDbContext appDbContext, IWebHostEnvironment webHostEnvironment) : Hub
{
    private readonly AppDbContext _appDbContext = appDbContext;
    private readonly IWebHostEnvironment _webHostEnvironment = webHostEnvironment;

    public async IAsyncEnumerable<byte[]> AudioStream(long fileId)
    {
        var ct = Context.ConnectionAborted;

        var song = await _appDbContext.Songs.Where(x => x.Id == fileId).SingleOrDefaultAsync(ct);
        if (song == null || !File.Exists(song.SongPath))
        {
            yield break;
        }

        using var stream = new FileStream(song.SongPath, FileMode.Open, FileAccess.Read);
        var buffer = new byte[1024 * 1024];
        int bytesRead;

        while ((bytesRead = await stream.ReadAsync(buffer, ct)) > 0)
        {
            yield return buffer.Take(bytesRead).ToArray();
        }

        song.ListeningCount++;
        await _appDbContext.SaveChangesAsync(ct);
    }
}
