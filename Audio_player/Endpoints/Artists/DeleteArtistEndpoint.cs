using Audio_player.Constants;
using Audio_player.Services;
using FastEndpoints;

namespace Audio_player.Endpoints.Artists;

public class DeleteArtistEndpoint(ArtistService artistService) : EndpointWithoutRequest
{
    private readonly ArtistService _artistService = artistService;

    public override void Configure()
    {
        Delete("{id:int}");
        Group<ArtistGroup>();
        Policies(PolicyNames.HasAdminRole);
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var id = Route<long>("id");

        if (!await _artistService.DeleteAsync(id, ct))
        {
            await SendNotFoundAsync(ct);
            return;
        }

        await SendOkAsync(ct);
    }
}
