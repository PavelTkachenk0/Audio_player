using FastEndpoints;

namespace Audio_player.Endpoints.Recommendations;

public class RecommendationsGroup : Group
{
    public RecommendationsGroup()
    {
        Configure("recommendations", ep => { });
    }
}
