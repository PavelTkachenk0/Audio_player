using Audio_player.Models.DTOs;

namespace Audio_player.Models.Responses;

public class GetRecommendationAlbumsResponse : BaseListResponse<AlbumDTO>
{
    public long TotalCount { get; set; }
}
