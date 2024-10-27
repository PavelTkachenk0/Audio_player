namespace Audio_player.Models.Requests;

public class PutArtistRequest
{
    public string ArtistName { get; set; } = null!;
    public IFormFile? Cover { get; set; }
    public IFormFile? Avatar { get; set; }
    public List<short> GenreIds { get; set; } = [];
}
