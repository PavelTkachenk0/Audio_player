namespace Audio_player.Models.Requests;

public class EditGenreRequest : BasePostImageRequest
{
    public string Name { get; set; } = null!;
}
