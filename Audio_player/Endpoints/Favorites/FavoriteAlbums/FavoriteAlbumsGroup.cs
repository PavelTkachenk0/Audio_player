using FastEndpoints;

namespace Audio_player.Endpoints.Favorites.FavoriteAlbums;

public class FavoriteAlbumsGroup : SubGroup<FavoritesGroup>
{
    public FavoriteAlbumsGroup()
    {
        Configure("albums", ep => { });
    }
}
