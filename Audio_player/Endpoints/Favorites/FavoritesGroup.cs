using Audio_player.Constants;
using FastEndpoints;

namespace Audio_player.Endpoints.Favorites;

public class FavoritesGroup : Group
{
    public FavoritesGroup()
    {
        Configure("favorites", ep =>
        {
            ep.Policies(PolicyNames.HasAdminOrUserRole);
        });
    }
}
