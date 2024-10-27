using Microsoft.AspNetCore.Mvc;

namespace Audio_player.Models.Requests;

public class PostArtistRequest : BasePostImageRequest
{
    public string ArtistName { get; set; } = null!;
    public short GenreId { get; set; }
    [FromForm(Name = "cover")]
    public IFormFile Cover { get; set; } = null!;

    [FromForm(Name = "avatar")]
    public IFormFile Avatar { get; set; } = null!;
}
