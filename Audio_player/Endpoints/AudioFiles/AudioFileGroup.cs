using FastEndpoints;

namespace Audio_player.Endpoints.Files;

public class AudioFileGroup : Group
{
    public AudioFileGroup()
    {
        Configure("audio-files", ep => { });
    }
}
