using FastEndpoints;

namespace Audio_player.Endpoints.Tracks;

public class TrackGroup : Group
{
    public TrackGroup()
    {
        Configure("tracks", ep => { });
    }
}
