using Audio_player.Models.Responses;
using Audio_player.Services;
using FastEndpoints;

namespace Audio_player.Endpoints.Authentication;

public class RefreshTokenEndpoint(AuthService authService) : EndpointWithoutRequest<TokenResponse>
{
    private readonly AuthService _authService = authService;

    public override void Configure()
    {
        Post("refresh");
        Group<AuthenticationGroup>();
        AllowAnonymous();
    }

    public override async Task<TokenResponse> ExecuteAsync(CancellationToken ct)
    {
        var refreshToken = HttpContext.Request.Cookies["refreshToken"];

        var result = await _authService.RefreshAsync(refreshToken, HttpContext.Response, ct);

        if (result == null)
        {
            ThrowError("Invalid or expired refresh token");
        }

        return result;
    }
}
