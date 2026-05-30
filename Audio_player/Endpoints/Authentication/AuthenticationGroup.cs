using FastEndpoints;

namespace Audio_player.Endpoints.Authentication;

public class AuthenticationGroup : Group
{
    public AuthenticationGroup()
    {
        Configure("auth", ep => { });
    }
}
