using Audio_player.Constants;
using Audio_player.Services;
using FastEndpoints;

namespace Audio_player.Endpoints.Albums;

public class DeleteAlbumEndpoint(AlbumService albumService) : EndpointWithoutRequest
{
    private readonly AlbumService _albumService = albumService;

    public override void Configure()
    {
        Delete("{id:int}");
        Group<AlbumGroup>();
        Policies(PolicyNames.HasAdminRole);
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var id = Route<long>("id");

        if (!await _albumService.DeleteAsync(id, ct))
        {
            await SendNotFoundAsync(ct);
            return;
        }

        await SendOkAsync(ct);
    }
}
