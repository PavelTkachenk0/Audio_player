using Audio_player.Constants;
using Audio_player.Models.Requests;
using Audio_player.Services;
using FastEndpoints;

namespace Audio_player.Endpoints.Artists;

public class PostArtistEndpoint(ArtistService artistService) : Endpoint<PostArtistRequest>
{
    private readonly ArtistService _artistService = artistService;

    public override void Configure()
    {
        Post("");
        Group<ArtistGroup>();
        AllowFileUploads();
        Policies(PolicyNames.HasAdminRole);
    }

    public override async Task HandleAsync(PostArtistRequest req, CancellationToken ct)
    {
        await _artistService.CreateAsync(req, ct);
        await SendOkAsync(ct);
    }
}
