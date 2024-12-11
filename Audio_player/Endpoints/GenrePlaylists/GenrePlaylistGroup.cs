using FastEndpoints;

namespace Audio_player.Endpoints.GenrePlaylists;

public class GenrePlaylistGroup : Group
{
    public GenrePlaylistGroup()
    {
        Configure("genre-playlists", ep => { });
    }
}
