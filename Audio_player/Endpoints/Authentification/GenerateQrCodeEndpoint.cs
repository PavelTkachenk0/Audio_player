using Audio_player.Constants;
using Audio_player.DAL;
using Audio_player.Models.Responses;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using OtpNet;
using QRCoder;
using System.Security.Claims;

namespace Audio_player.Endpoints.Authentification;

public class GenerateQrCodeEndpoint(AppDbContext appDbContext) : EndpointWithoutRequest<GenerateQrCodeResponse>
{
    private readonly AppDbContext _appDbContext = appDbContext;

    public override void Configure()
    {
        Post("generate-qr-code");
        Group<AuthentificationGroup>();
        Policies(PolicyNames.HasAdminOrUserRole);
    }

    public override async Task<GenerateQrCodeResponse> ExecuteAsync(CancellationToken ct)
    {
        var email = HttpContext.User.Claims.
          FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value!;
        var user = await _appDbContext.AppUsers.Where(x => x.Email == email)
                .SingleOrDefaultAsync(ct);

        if (user == null)
        {
            await SendNotFoundAsync(ct);
            ThrowError("user_is_not_found");
        }

        if (!user.IsTwoFactorEnable)
        {
            await SendErrorsAsync(400, ct);
            ThrowError("2fa_disabled");
        }

        var secretKey = KeyGeneration.GenerateRandomKey(20);
        user.TwoFactorSecret = Base32Encoding.ToString(secretKey);

        _appDbContext.AppUsers.Update(user);
        await _appDbContext.SaveChangesAsync(ct);

        var otpAuthUri = new OtpUri(
            OtpType.Totp,
            user.TwoFactorSecret,
            user.Email,
            "Audio_player");

        using var qrGenerator = new QRCodeGenerator();
        using var qrData = qrGenerator.CreateQrCode(otpAuthUri.ToString(), QRCodeGenerator.ECCLevel.Q);
        using var qrCode = new PngByteQRCode(qrData);
        var qrCodeImage = qrCode.GetGraphic(20);

        return new GenerateQrCodeResponse
        {
            QrCodeImageBase64 = $"data:image/png;base64,{Convert.ToBase64String(qrCodeImage)}",
            SecretKey = user.TwoFactorSecret
        };
    }
}
