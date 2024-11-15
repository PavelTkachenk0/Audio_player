using FastEndpoints;

namespace Audio_player.Endpoints.Favorites.FavoritePlaylists;

public class FavoritePlaylistGroup : SubGroup<FavoritesGroup>
{
    public FavoritePlaylistGroup()
    {
        Configure("playlists", ep => { });
    }
}
