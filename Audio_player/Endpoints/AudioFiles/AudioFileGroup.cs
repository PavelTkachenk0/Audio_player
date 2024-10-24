using Audio_player.Constants;
using FastEndpoints;

namespace Audio_player.Endpoints.Files;

public class AudioFileGroup : Group
{
    public AudioFileGroup()
    {
        Configure("audio-files", ep =>
        {
            ep.Policies(PolicyNames.HasAdminRole);
        });
    }
}
