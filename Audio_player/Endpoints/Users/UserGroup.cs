using FastEndpoints;

namespace Audio_player.Endpoints.Users;

public class UserGroup : Group
{
    public UserGroup()
    {
        Configure("users", ep => { });
    }
}
