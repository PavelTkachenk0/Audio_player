using Audio_player.Constants;
using Audio_player.Models.Responses;
using Audio_player.Services;
using FastEndpoints;

namespace Audio_player.Endpoints.Authentication;

public class GenerateQrCodeEndpoint(AuthService authService) : EndpointWithoutRequest<GenerateQrCodeResponse>
{
    private readonly AuthService _authService = authService;

    public override void Configure()
    {
        Post("generate-qr-code");
        Group<AuthenticationGroup>();
        Policies(PolicyNames.HasAdminOrUserRole);
    }

    public override async Task<GenerateQrCodeResponse> ExecuteAsync(CancellationToken ct)
    {
        var (status, image) = await _authService.GenerateQrCodeAsync(HttpContext.User, ct);

        if (status == GenerateQrResult.UserNotFound)
        {
            ThrowError("user_is_not_found");
        }

        if (status == GenerateQrResult.TwoFactorDisabled)
        {
            ThrowError("2fa_disabled");
        }

        return new GenerateQrCodeResponse { QrCodeImageBase64 = image! };
    }
}
