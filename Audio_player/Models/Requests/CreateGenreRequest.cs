namespace Audio_player.Models.Requests;

public class CreateGenreRequest : BasePostImageRequest
{
    public string Name { get; set; } = null!;
}
