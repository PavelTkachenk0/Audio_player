using FastEndpoints;

namespace Audio_player.Endpoints.Favorites.FavoritePlaylists;

public class FavoritePlaylistsGroup : SubGroup<FavoritesGroup>
{
    public FavoritePlaylistsGroup()
    {
        Configure("playlists", ep => { });
    }
}
