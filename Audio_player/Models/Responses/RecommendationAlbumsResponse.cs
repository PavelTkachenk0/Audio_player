using Audio_player.Models.DTOs;

namespace Audio_player.Models.Responses;

public class RecommendationAlbumsResponse : BaseListResponse<AlbumDTO>
{
    public long TotalCount { get; set; }
}
