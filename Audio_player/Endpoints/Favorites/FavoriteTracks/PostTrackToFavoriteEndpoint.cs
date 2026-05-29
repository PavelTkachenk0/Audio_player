using Audio_player.Endpoints.Favorites.Tracks;
using Audio_player.Models.Responses;
using Audio_player.Services;
using FastEndpoints;

namespace Audio_player.Endpoints.Favorites.FavoriteTracks;

public class PostTrackToFavoriteEndpoint(FavoriteService favoriteService) : EndpointWithoutRequest<FavoriteResponse>
{
    private readonly FavoriteService _favoriteService = favoriteService;

    public override void Configure()
    {
        Post("{id:int}");
        Group<FavoriteTracksGroup>();
    }

    public override async Task<FavoriteResponse> ExecuteAsync(CancellationToken ct)
    {
        var trackId = Route<long>("id");

        if (!await _favoriteService.AddTrackAsync(trackId, HttpContext.User, ct))
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
