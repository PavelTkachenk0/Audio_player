using Audio_player.Models.Responses;
using Audio_player.Services;
using FastEndpoints;

namespace Audio_player.Endpoints.Favorites.FavoriteArtists;

public class GetFavoriteArtistsEndpoint(FavoriteService favoriteService) : EndpointWithoutRequest<GetFavoriteArtistsResponse>
{
    private readonly FavoriteService _favoriteService = favoriteService;

    public override void Configure()
    {
        Get("");
        Group<FavoriteArtistsGroup>();
    }

    public override async Task<GetFavoriteArtistsResponse> ExecuteAsync(CancellationToken ct)
    {
        var artists = await _favoriteService.GetFavoriteArtistsAsync(HttpContext.User, ct);

        return new GetFavoriteArtistsResponse
        {
            Result = artists
        };
    }
}
