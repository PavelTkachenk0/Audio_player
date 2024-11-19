namespace Audio_player.Models.Requests;

public class PostAlbumRequest
{
    public string AlbumName { get; set; } = null!;
    public IFormFile Cover { get; set; } = null!;
    public List<short> GenreIds { get; set; } = [];
    public List<long> ArtistIds { get; set; } = [];
}
