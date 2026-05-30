using Audio_player.Constants;
using Audio_player.Models.Responses;
using Audio_player.Services;
using FastEndpoints;

namespace Audio_player.Endpoints.Tracks;

public class GetTrackByAlbumIdEndpoint(TrackService trackService) : EndpointWithoutRequest<GetTracksByAlbumIdResponse>
{
    private readonly TrackService _trackService = trackService;

    public override void Configure()
    {
        Get("by-albumId/{albumId:int}");
        Group<TrackGroup>();
        Policies(PolicyNames.HasAdminOrUserRole);
    }

    public override async Task<GetTracksByAlbumIdResponse> ExecuteAsync(CancellationToken ct)
    {
        var albumId = Route<long>("albumId");

        var tracks = await _trackService.GetByAlbumIdAsync(albumId, HttpContext.User, ct);

        return new GetTracksByAlbumIdResponse
        {
            Result = tracks
        };
    }
}
