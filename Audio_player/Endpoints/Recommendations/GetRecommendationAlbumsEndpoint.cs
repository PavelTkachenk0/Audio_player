using Audio_player.Constants;
using Audio_player.Models.Requests;
using Audio_player.Models.Responses;
using Audio_player.Services;
using FastEndpoints;

namespace Audio_player.Endpoints.Recommendations;

public class GetRecommendationAlbumsEndpointI(RecommendationService recommendationService) : Endpoint<RecommendationRequest, RecommendationAlbumsResponse>
{
    private readonly RecommendationService _recommendationService = recommendationService;

    public override void Configure()
    {
        Get("albums");
        Group<RecommendationsGroup>();
        Policies(PolicyNames.HasAdminOrUserRole);
    }

    public override async Task<RecommendationAlbumsResponse> ExecuteAsync(RecommendationRequest req, CancellationToken ct)
    {
        var (result, totalCount) = await _recommendationService.GetAlbumsAsync(req, HttpContext.User, ct);

        return new RecommendationAlbumsResponse
        {
            Result = result,
            TotalCount = totalCount
        };
    }
}
