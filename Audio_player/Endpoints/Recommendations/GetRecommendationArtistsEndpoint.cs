using Audio_player.Constants;
using Audio_player.Models.Requests;
using Audio_player.Models.Responses;
using Audio_player.Services;
using FastEndpoints;

namespace Audio_player.Endpoints.Recommendations;

public class GetRecommendationArtistsEndpoint(RecommendationService recommendationService) : Endpoint<RecommendationRequest, GetRecommendationArtistsResponse>
{
    private readonly RecommendationService _recommendationService = recommendationService;

    public override void Configure()
    {
        Get("artists");
        Group<RecommendationsGroup>();
        Policies(PolicyNames.HasAdminOrUserRole);
    }

    public override async Task<GetRecommendationArtistsResponse> ExecuteAsync(RecommendationRequest req, CancellationToken ct)
    {
        var (artists, totalCount) = await _recommendationService.GetArtistsAsync(req, HttpContext.User, ct);

        return new GetRecommendationArtistsResponse
        {
            Result = artists,
            TotalCount = totalCount
        };
    }
}
