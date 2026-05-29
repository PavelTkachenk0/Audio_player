using Audio_player.Constants;
using Audio_player.Models.Requests;
using Audio_player.Models.Responses;
using Audio_player.Services;
using FastEndpoints;

namespace Audio_player.Endpoints.Recommendations;

public class GetRecommendationTracksEndpoint(RecommendationService recommendationService) : Endpoint<RecommendationRequest, GetRecommendationTracksResponse>
{
    private readonly RecommendationService _recommendationService = recommendationService;

    public override void Configure()
    {
        Get("tracks");
        Group<RecommendationsGroup>();
        Policies(PolicyNames.HasAdminOrUserRole);
    }

    public override async Task<GetRecommendationTracksResponse> ExecuteAsync(RecommendationRequest req, CancellationToken ct)
    {
        var (tracks, totalCount) = await _recommendationService.GetTracksAsync(req, HttpContext.User, ct);

        return new GetRecommendationTracksResponse
        {
            Result = tracks,
            TotalCount = totalCount
        };
    }
}
