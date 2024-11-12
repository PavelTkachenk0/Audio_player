using Audio_player.Models.DTOs;

namespace Audio_player.Models.Responses;

public class GetRecommendationTracksResponse : BaseListResponse<TrackDTO>
{
    public long TotalCount { get; set; }
}
