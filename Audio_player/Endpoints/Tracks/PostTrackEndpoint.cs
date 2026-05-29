using Audio_player.Constants;
using Audio_player.Models.Requests;
using Audio_player.Services;
using FastEndpoints;

namespace Audio_player.Endpoints.Tracks;

public class PostTrackEndpoint(TrackService trackService) : Endpoint<PostTrackRequest>
{
    private readonly TrackService _trackService = trackService;

    public override void Configure()
    {
        Post("");
        Group<TrackGroup>();
        AllowFileUploads();
        Policies(PolicyNames.HasAdminRole);
    }

    public override async Task HandleAsync(PostTrackRequest req, CancellationToken ct)
    {
        await _trackService.CreateAsync(req, ct);
        await SendOkAsync(ct);
    }
}
