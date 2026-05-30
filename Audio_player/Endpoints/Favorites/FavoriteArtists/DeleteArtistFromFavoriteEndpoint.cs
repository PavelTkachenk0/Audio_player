using Audio_player.Models.Responses;
using Audio_player.Services;
using FastEndpoints;

namespace Audio_player.Endpoints.Favorites.FavoriteArtists;

public class DeleteArtistFromFavoriteEndpoint(FavoriteService favoriteService) : EndpointWithoutRequest<FavoriteResponse>
{
    private readonly FavoriteService _favoriteService = favoriteService;

    public override void Configure()
    {
        Delete("{id:int}");
        Group<FavoriteArtistsGroup>();
    }

    public override async Task<FavoriteResponse> ExecuteAsync(CancellationToken ct)
    {
        var artistId = Route<long>("id");

        if (!await _favoriteService.RemoveArtistAsync(artistId, HttpContext.User, ct))
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
