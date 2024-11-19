using FastEndpoints;

namespace Audio_player.Endpoints.Search;

public class SearchGroup : Group
{
    public SearchGroup()
    {
        Configure("search", ep => { });
    }
}
