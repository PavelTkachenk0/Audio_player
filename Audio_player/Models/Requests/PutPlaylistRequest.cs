namespace Audio_player.Models.Requests;

public class PutPlaylistRequest
{
    public string Name { get; set; } = null!;
    public IFormFile? Cover { get; set; } 
    public List<long> TrackIds { get; set; } = [];
}
