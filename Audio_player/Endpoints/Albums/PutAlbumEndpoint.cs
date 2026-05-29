using Audio_player.Constants;
using Audio_player.Models.Requests;
using Audio_player.Services;
using FastEndpoints;

namespace Audio_player.Endpoints.Albums;

public class PutAlbumEndpoint(AlbumService albumService) : Endpoint<PutAlbumRequest>
{
    private readonly AlbumService _albumService = albumService;

    public override void Configure()
    {
        Put("{id:int}");
        Group<AlbumGroup>();
        AllowFileUploads();
        Policies(PolicyNames.HasAdminRole);
    }

    public override async Task HandleAsync(PutAlbumRequest req, CancellationToken ct)
    {
        var id = Route<long>("id");

        if (!await _albumService.UpdateAsync(id, req, ct))
        {
            await SendNotFoundAsync(ct);
            return;
        }

        await SendOkAsync(ct);
    }
}
