namespace Audio_player.Models.Requests;

public class PutAlbumRequest
{
    public string AlbumName { get; set; } = null!;
    public IFormFile? Cover { get; set; }
    public List<short> GenreIds { get; set; } = [];
    public List<long> ArtistIds { get; set; } = [];
}
