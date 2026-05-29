using Audio_player.Constants;
using Audio_player.Models.Requests;
using Audio_player.Models.Responses;
using Audio_player.Services;
using FastEndpoints;

namespace Audio_player.Endpoints.Artists;

public class GetArtistsByNameEndpoint(ArtistService artistService) : Endpoint<GetByNameRequest, GetArtistsResponse>
{
    private readonly ArtistService _artistService = artistService;

    public override void Configure()
    {
        Get("");
        Group<ArtistGroup>();
        Policies(PolicyNames.HasAdminOrUserRole);
    }

    public override async Task<GetArtistsResponse> ExecuteAsync(GetByNameRequest req, CancellationToken ct)
    {
        var artists = await _artistService.GetByNameAsync(req.Name, HttpContext.User, ct);

        return new GetArtistsResponse
        {
            Result = artists
        };
    }
}
