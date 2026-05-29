using Audio_player.Models.Responses;
using Audio_player.Services;
using FastEndpoints;

namespace Audio_player.Endpoints.Favorites.FavoriteAlbums;

public class PostAlbumToFavoriteEndpoint(FavoriteService favoriteService) : EndpointWithoutRequest<FavoriteResponse>
{
    private readonly FavoriteService _favoriteService = favoriteService;

    public override void Configure()
    {
        Post("{id:int}");
        Group<FavoriteAlbumsGroup>();
    }

    public override async Task<FavoriteResponse> ExecuteAsync(CancellationToken ct)
    {
        var albumId = Route<long>("id");

        if (!await _favoriteService.AddAlbumAsync(albumId, HttpContext.User, ct))
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
