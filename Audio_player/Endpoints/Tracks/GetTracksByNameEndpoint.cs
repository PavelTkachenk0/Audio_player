using Audio_player.Constants;
using Audio_player.Models.Requests;
using Audio_player.Models.Responses;
using Audio_player.Services;
using FastEndpoints;

namespace Audio_player.Endpoints.Tracks;

public class GetTracksByNameEndpoint(TrackService trackService) : Endpoint<GetByNameRequest, GetTracksResponse>
{
    private readonly TrackService _trackService = trackService;

    public override void Configure()
    {
        Get("");
        Group<TrackGroup>();
        Policies(PolicyNames.HasAdminOrUserRole);
    }

    public override async Task<GetTracksResponse> ExecuteAsync(GetByNameRequest req, CancellationToken ct)
    {
        var tracks = await _trackService.GetByNameAsync(req.Name, HttpContext.User, ct);

        return new GetTracksResponse
        {
            Result = tracks
        };
    }
}
