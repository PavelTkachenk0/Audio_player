using Audio_player.Constants;
using Audio_player.Models.Requests;
using Audio_player.Models.Responses;
using Audio_player.Services;
using FastEndpoints;

namespace Audio_player.Endpoints.Authentication;

public class EnableTwoFactorAuthenticationEndpoint(AuthService authService) : Endpoint<EnableTwoFactorAuthenticationRequest, ConfirmResponse>
{
    private readonly AuthService _authService = authService;

    public override void Configure()
    {
        Put("/enable-2fa");
        Group<AuthenticationGroup>();
        Policies(PolicyNames.HasAdminOrUserRole);
    }

    public override async Task<ConfirmResponse> ExecuteAsync(EnableTwoFactorAuthenticationRequest req, CancellationToken ct)
    {
        if (!await _authService.SetTwoFactorAsync(HttpContext.User, req.Enable, ct))
        {
            ThrowError("user_is_not_found");
        }

        return new ConfirmResponse { Success = true };
    }
}
