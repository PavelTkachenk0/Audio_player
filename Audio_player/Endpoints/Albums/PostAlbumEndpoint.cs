using Audio_player.Constants;
using Audio_player.Models.Requests;
using Audio_player.Services;
using FastEndpoints;

namespace Audio_player.Endpoints.Albums;

public class PostAlbumEndpoint(AlbumService albumService) : Endpoint<PostAlbumRequest>
{
    private readonly AlbumService _albumService = albumService;

    public override void Configure()
    {
        Post("");
        Group<AlbumGroup>();
        AllowFileUploads();
        Policies(PolicyNames.HasAdminRole);
    }

    public override async Task HandleAsync(PostAlbumRequest req, CancellationToken ct)
    {
        await _albumService.CreateAsync(req, ct);
        await SendOkAsync(ct);
    }
}
