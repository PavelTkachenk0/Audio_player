using Audio_player.Constants;
using Audio_player.DAL;
using Audio_player.Services;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;

namespace Audio_player.Endpoints.GenrePlaylists;

public class DeleteGenrePlaylistEndpoint(AppDbContext appDbContext, FileService fileService) : EndpointWithoutRequest
{
    private readonly AppDbContext _appDbContext = appDbContext;
    private readonly FileService _fileService = fileService;

    public override void Configure()
    {
        Delete("{id:int}");
        Group<GenrePlaylistGroup>();
        Policies(PolicyNames.HasAdminRole);
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var id = Route<long>("id");

        var playlist = await _appDbContext.Playlists.SingleOrDefaultAsync(x => x.Id == id, ct);

        if (playlist == null)
        {
            await SendNotFoundAsync(ct);
            return;
        }

        _appDbContext.Playlists.Remove(playlist);

        _fileService.DeleteFile(playlist.CoverPath);

        await _appDbContext.SaveChangesAsync(ct);
        await SendOkAsync(ct);
    }
}
