using Audio_player.Constants;
using Audio_player.Models.Requests;
using Audio_player.Models.Responses;
using Audio_player.Services;
using FastEndpoints;

namespace Audio_player.Endpoints.Recommendations;

public class GetRecommendationPlaylistsEndpoint(RecommendationService recommendationService) : Endpoint<RecommendationRequest, GetRecommendationPlaylistsResposne>
{
    private readonly RecommendationService _recommendationService = recommendationService;

    public override void Configure()
    {
        Get("playlists");
        Group<RecommendationsGroup>();
        Policies(PolicyNames.HasAdminOrUserRole);
    }

    public override async Task<GetRecommendationPlaylistsResposne> ExecuteAsync(RecommendationRequest req, CancellationToken ct)
    {
        var (playlists, totalCount) = await _recommendationService.GetPlaylistsAsync(req, HttpContext.User, ct);

        return new GetRecommendationPlaylistsResposne
        {
            Result = playlists,
            TotalCount = totalCount
        };
    }
}
