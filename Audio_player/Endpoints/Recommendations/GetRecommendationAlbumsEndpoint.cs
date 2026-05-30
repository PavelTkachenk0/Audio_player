using Audio_player.Constants;
using Audio_player.Models.Requests;
using Audio_player.Models.Responses;
using Audio_player.Services;
using FastEndpoints;

namespace Audio_player.Endpoints.Recommendations;

public class GetRecommendationAlbumsEndpoint(RecommendationService recommendationService) : Endpoint<RecommendationRequest, GetRecommendationAlbumsResponse>
{
    private readonly RecommendationService _recommendationService = recommendationService;

    public override void Configure()
    {
        Get("albums");
        Group<RecommendationsGroup>();
        Policies(PolicyNames.HasAdminOrUserRole);
    }

    public override async Task<GetRecommendationAlbumsResponse> ExecuteAsync(RecommendationRequest req, CancellationToken ct)
    {
        var (result, totalCount) = await _recommendationService.GetAlbumsAsync(req, HttpContext.User, ct);

        return new GetRecommendationAlbumsResponse
        {
            Result = result,
            TotalCount = totalCount
        };
    }
}
