using Audio_player.Endpoints.Favorites.Tracks;
using Audio_player.Models.Responses;
using Audio_player.Services;
using FastEndpoints;

namespace Audio_player.Endpoints.Favorites.FavoriteTracks;

public class GetFavoriteTracksEndpoint(FavoriteService favoriteService) : EndpointWithoutRequest<GetFavoriteTracksResponse>
{
    private readonly FavoriteService _favoriteService = favoriteService;

    public override void Configure()
    {
        Get("");
        Group<FavoriteTracksGroup>();
    }

    public override async Task<GetFavoriteTracksResponse> ExecuteAsync(CancellationToken ct)
    {
        var tracks = await _favoriteService.GetFavoriteTracksAsync(HttpContext.User, ct);

        return new GetFavoriteTracksResponse
        {
            Result = tracks
        };
    }
}
