using Audio_player.Models.Requests;
using Audio_player.Models.Responses;
using Audio_player.Services;
using FastEndpoints;

namespace Audio_player.Endpoints.Authentication;

public class LoginEndpoint(AuthService authService) : Endpoint<LoginRequest, TokenResponse>
{
    private readonly AuthService _authService = authService;

    public override void Configure()
    {
        Post("login");
        Group<AuthenticationGroup>();
        AllowAnonymous();
    }

    public override async Task<TokenResponse> ExecuteAsync(LoginRequest req, CancellationToken ct)
    {
        var result = await _authService.LoginAsync(req, HttpContext.Response, ct);

        if (result == null)
        {
            ThrowError("invalid_login_or_password");
        }

        return result;
    }
}
