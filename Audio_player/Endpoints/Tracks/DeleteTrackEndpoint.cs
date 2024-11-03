using Audio_player.Constants;
using Audio_player.DAL;
using Audio_player.Services;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;

namespace Audio_player.Endpoints.Tracks;

public class DeleteTrackEndpoint(AppDbContext appDbContext, FileService fileService) : EndpointWithoutRequest
{
    private readonly AppDbContext _appDbContext = appDbContext;
    private readonly FileService _fileService = fileService;

    public override void Configure()
    {
        Delete("{id:int}");
        Group<TrackGroup>();
        Policies(PolicyNames.HasAdminRole);
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var id = Route<long>("id");

        var track = await _appDbContext.Songs.SingleOrDefaultAsync(x => x.Id == id, ct);

        if (track == null)
        {
            await SendNotFoundAsync(ct);
            return;
        }

        _appDbContext.Songs.Remove(track);

        _fileService.DeleteFile(track.SongPath);

        await _appDbContext.SaveChangesAsync(ct);

        await SendOkAsync(ct);
    }
}
