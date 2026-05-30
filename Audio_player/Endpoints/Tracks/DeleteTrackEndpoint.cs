using Audio_player.Constants;
using Audio_player.Services;
using FastEndpoints;

namespace Audio_player.Endpoints.Tracks;

public class DeleteTrackEndpoint(TrackService trackService) : EndpointWithoutRequest
{
    private readonly TrackService _trackService = trackService;

    public override void Configure()
    {
        Delete("{id:int}");
        Group<TrackGroup>();
        Policies(PolicyNames.HasAdminRole);
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var id = Route<long>("id");

        if (!await _trackService.DeleteAsync(id, ct))
        {
            await SendNotFoundAsync(ct);
            return;
        }

        await SendOkAsync(ct);
    }
}
