namespace Audio_player.Models.Responses;

public class TokenResponse
{
    public string? AccessToken { get; set; }
    public bool RequiresTwoFactor { get; set; }
    public string? TwoFactorToken { get; set; }
}
