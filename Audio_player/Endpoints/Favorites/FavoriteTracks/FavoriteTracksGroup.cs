using FastEndpoints;

namespace Audio_player.Endpoints.Favorites.Tracks;

public class FavoriteTracksGroup : SubGroup<FavoritesGroup>
{
    public FavoriteTracksGroup()
    {
        Configure("tracks", ep => { });
    }
}
