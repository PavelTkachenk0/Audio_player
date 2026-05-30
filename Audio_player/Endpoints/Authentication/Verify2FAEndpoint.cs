using Audio_player.Models.Requests;
using Audio_player.Models.Responses;
using Audio_player.Services;
using FastEndpoints;

namespace Audio_player.Endpoints.Authentication;

public class Verify2FAEndpoint(AuthService authService) : Endpoint<Verify2FARequest, TokenResponse?>
{
    private readonly AuthService _authService = authService;

    public override void Configure()
    {
        Post("verify-2fa");
        Group<AuthenticationGroup>();
        AllowAnonymous();
    }

    public override async Task<TokenResponse?> ExecuteAsync(Verify2FARequest req, CancellationToken ct)
    {
        var result = await _authService.VerifyTwoFactorAsync(req, HttpContext.Response, ct);

        if (result == null)
        {
            await SendUnauthorizedAsync(ct);
            return null;
        }

        return result;
    }
}
