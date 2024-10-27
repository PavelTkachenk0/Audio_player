using FastEndpoints;

namespace Audio_player.Endpoints.Artists;

public class ArtistGroup : Group
{
    public ArtistGroup()
    {
        Configure("artists", ep => { });
    }
}
