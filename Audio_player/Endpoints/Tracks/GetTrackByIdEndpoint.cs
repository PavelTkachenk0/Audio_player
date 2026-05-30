using Audio_player.Constants;
using Audio_player.Models.DTOs;
using Audio_player.Services;
using FastEndpoints;

namespace Audio_player.Endpoints.Tracks;

public class GetTrackByIdEndpoint(TrackService trackService) : EndpointWithoutRequest<TrackDTO?>
{
    private readonly TrackService _trackService = trackService;

    public override void Configure()
    {
        Get("{id:int}");
        Group<TrackGroup>();
        Policies(PolicyNames.HasAdminOrUserRole);
    }

    public override async Task<TrackDTO?> ExecuteAsync(CancellationToken ct)
    {
        var id = Route<long>("id");

        var track = await _trackService.GetByIdAsync(id, HttpContext.User, ct);

        if (track == null)
        {
            await SendNotFoundAsync(ct);
            return null;
        }

        return track;
    }
}
