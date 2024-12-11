namespace Audio_player.Models.Requests;

public class PostTrackRequest
{
    public string SongName { get; set; } = null!;
    public IFormFile SongFile { get; set; } = null!;
    public long AlbumId { get; set; }
    public List<long> ArtistIds { get; set; } = [];
    public List<short> GenreIds { get; set; } = [];
    public int Duration { get; set; }
}
