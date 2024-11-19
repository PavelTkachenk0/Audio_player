using Audio_player.Models.DTOs;

namespace Audio_player.Models.Responses;

public class GetRecommendationPlaylistsResposne : BaseListResponse<ShortGenrePlaylistDTO>
{
    public long TotalCount { get; set; }
}
