namespace Audio_player.Models.Requests;

public class Verify2FARequest
{
    public string Email { get; set; } = null!;
    public string Code { get; set; } = null!;
}
