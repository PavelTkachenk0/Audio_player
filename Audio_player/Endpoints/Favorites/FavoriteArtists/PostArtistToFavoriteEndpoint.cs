using Audio_player.Models.Responses;
using Audio_player.Services;
using FastEndpoints;

namespace Audio_player.Endpoints.Favorites.FavoriteArtists;

public class PostArtistToFavoriteEndpoint(FavoriteService favoriteService) : EndpointWithoutRequest<FavoriteResponse>
{
    private readonly FavoriteService _favoriteService = favoriteService;

    public override void Configure()
    {
        Post("{id:int}");
        Group<FavoriteArtistsGroup>();
    }

    public override async Task<FavoriteResponse> ExecuteAsync(CancellationToken ct)
    {
        var artistId = Route<long>("id");

        if (!await _favoriteService.AddArtistAsync(artistId, HttpContext.User, ct))
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
