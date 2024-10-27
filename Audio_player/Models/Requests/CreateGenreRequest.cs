using Microsoft.AspNetCore.Mvc;

namespace Audio_player.Models.Requests;

public class CreateGenreRequest 
{
    public string Name { get; set; } = null!;
    [FromForm(Name = "cover")]
    public IFormFile Cover { get; set; } = null!;
}
