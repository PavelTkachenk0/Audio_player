using Audio_player.Models.DTOs;

namespace Audio_player.Models.Responses;

public class GetRecommendationArtistsResponse : BaseListResponse<ArtistDTO>
{
    public long TotalCount { get; set; }
}
