using Audio_player.Models.Responses;
using Audio_player.Services;
using FastEndpoints;

namespace Audio_player.Endpoints.Favorites.FavoritePlaylists;

public class GetFavoritePlaylistsEndpoint(FavoriteService favoriteService) : EndpointWithoutRequest<GetFavoritePlaylistsResponse>
{
    private readonly FavoriteService _favoriteService = favoriteService;

    public override void Configure()
    {
        Get("");
        Group<FavoritePlaylistsGroup>();
    }

    public override async Task<GetFavoritePlaylistsResponse> ExecuteAsync(CancellationToken ct)
    {
        var playlists = await _favoriteService.GetFavoritePlaylistsAsync(HttpContext.User, ct);

        return new GetFavoritePlaylistsResponse
        {
            Result = playlists
        };
    }
}
