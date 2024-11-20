namespace Audio_player.Models.Responses;

public class GenerateQrCodeResponse
{
    public string QrCodeImageBase64 { get; set; } = null!;
    public string SecretKey { get; set; } = null!;
}
