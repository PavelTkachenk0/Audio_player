using Microsoft.AspNetCore.Mvc;

namespace Audio_player.Models.Requests;

public class PostArtistRequest 
{
    public string ArtistName { get; set; } = null!;
    [FromForm]
    public List<short> GenreIds { get; set; } = [];
    [FromForm(Name = "cover")]
    public IFormFile Cover { get; set; } = null!;

    [FromForm(Name = "avatar")]
    public IFormFile Avatar { get; set; } = null!;
}
