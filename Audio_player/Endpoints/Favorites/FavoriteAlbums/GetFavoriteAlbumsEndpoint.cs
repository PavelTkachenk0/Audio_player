using Audio_player.Models.Responses;
using Audio_player.Services;
using FastEndpoints;

namespace Audio_player.Endpoints.Favorites.FavoriteAlbums;

public class GetFavoriteAlbumsEndpoint(FavoriteService favoriteService) : EndpointWithoutRequest<GetFavoriteAlbumsResponse>
{
    private readonly FavoriteService _favoriteService = favoriteService;

    public override void Configure()
    {
        Get("");
        Group<FavoriteAlbumsGroup>();
    }

    public override async Task<GetFavoriteAlbumsResponse> ExecuteAsync(CancellationToken ct)
    {
        var albums = await _favoriteService.GetFavoriteAlbumsAsync(HttpContext.User, ct);

        return new GetFavoriteAlbumsResponse
        {
            Result = albums
        };
    }
}
