using FastEndpoints;

namespace Audio_player.Endpoints.Favorites.FavoriteTracks;

public class FavoriteTracksGroup : SubGroup<FavoritesGroup>
{
    public FavoriteTracksGroup()
    {
        Configure("tracks", ep => { });
    }
}
