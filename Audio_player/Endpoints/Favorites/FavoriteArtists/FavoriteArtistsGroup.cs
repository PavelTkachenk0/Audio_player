using FastEndpoints;

namespace Audio_player.Endpoints.Favorites.FavoriteArtists;

public class FavoriteArtistsGroup : SubGroup<FavoritesGroup>
{
    public FavoriteArtistsGroup()
    {
        Configure("artists", ep => { });
    }
}
