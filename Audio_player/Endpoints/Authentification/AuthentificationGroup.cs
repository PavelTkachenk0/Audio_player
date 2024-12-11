using FastEndpoints;

namespace Audio_player.Endpoints.Authentification;

public class AuthentificationGroup : Group
{
    public AuthentificationGroup()
    {
        Configure("auth", ep => { });
    }
}
