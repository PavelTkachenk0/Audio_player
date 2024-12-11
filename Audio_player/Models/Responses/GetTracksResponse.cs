using Audio_player.Models.DTOs;

namespace Audio_player.Models.Responses;

public class GetTracksResponse : BaseListResponse<TrackDTO>;

public class GetTracksByAlbumIdResponse : BaseListResponse<TrackByAlbumIdDTO>;
