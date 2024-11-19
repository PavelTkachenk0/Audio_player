using FastEndpoints;

namespace Audio_player.Endpoints.Albums;

public class AlbumGroup :  Group
{
    public AlbumGroup()
    {
        Configure("albums", ep => { });
    }
}
