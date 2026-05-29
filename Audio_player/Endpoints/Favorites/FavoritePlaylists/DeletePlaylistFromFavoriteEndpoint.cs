using Audio_player.Models.Responses;
using Audio_player.Services;
using FastEndpoints;

namespace Audio_player.Endpoints.Favorites.FavoritePlaylists;

public class DeletePlaylistFromFavoriteEndpoint(FavoriteService favoriteService) : EndpointWithoutRequest<FavoriteResponse>
{
    private readonly FavoriteService _favoriteService = favoriteService;

    public override void Configure()
    {
        Delete("{id:int}");
        Group<FavoritePlaylistGroup>();
    }

    public override async Task<FavoriteResponse> ExecuteAsync(CancellationToken ct)
    {
        var playlistId = Route<long>("id");

        if (!await _favoriteService.RemovePlaylistAsync(playlistId, HttpContext.User, ct))
        {
            await SendNotFoundAsync(ct);
            return new FavoriteResponse
            {
                Added = false
            };
        }

        return new FavoriteResponse
        {
            Added = true
        };
    }
}
