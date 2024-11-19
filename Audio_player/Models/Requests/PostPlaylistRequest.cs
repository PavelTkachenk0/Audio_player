namespace Audio_player.Models.Requests;

public class PostPlaylistRequest
{
    public string Name { get; set; } = null!;
    public IFormFile Cover { get; set; } = null!;
    public List<long> TrackIds { get; set; } = [];
}
