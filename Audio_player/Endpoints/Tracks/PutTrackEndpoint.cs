using Audio_player.Constants;
using Audio_player.Models.Requests;
using Audio_player.Services;
using FastEndpoints;

namespace Audio_player.Endpoints.Tracks;

public class PutTrackEndpoint(TrackService trackService) : Endpoint<PutTrackRequest>
{
    private readonly TrackService _trackService = trackService;

    public override void Configure()
    {
        Put("{id:int}");
        Group<TrackGroup>();
        AllowFileUploads();
        Policies(PolicyNames.HasAdminRole);
    }

    public override async Task HandleAsync(PutTrackRequest req, CancellationToken ct)
    {
        var id = Route<long>("id");

        if (!await _trackService.UpdateAsync(id, req, ct))
        {
            await SendNotFoundAsync(ct);
            return;
        }

        await SendOkAsync(ct);
    }
}
