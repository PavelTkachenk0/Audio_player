using Audio_player.Services;
using FastEndpoints;

namespace Audio_player.Endpoints.Authentication;

public class LogoutEndpoint(AuthService authService) : EndpointWithoutRequest
{
    private readonly AuthService _authService = authService;

    public override void Configure()
    {
        Post("logout");
        Group<AuthenticationGroup>();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var refreshToken = HttpContext.Request.Cookies["refreshToken"];
        var accessToken = HttpContext.Request.Headers.Authorization.ToString().Replace("Bearer ", "");

        await _authService.LogoutAsync(refreshToken, accessToken, HttpContext.Response, ct);

        await SendOkAsync(ct);
    }
}
