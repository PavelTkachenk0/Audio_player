using Audio_player.Models.Requests;
using Audio_player.Models.Responses;
using Audio_player.Services;
using FastEndpoints;

namespace Audio_player.Endpoints.Authentication;

public class RegisterEndpoint(AuthService authService) : Endpoint<RegisterRequest, TokenResponse>
{
    private readonly AuthService _authService = authService;

    public override void Configure()
    {
        Post("register");
        Group<AuthenticationGroup>();
        AllowAnonymous();
    }

    public override Task<TokenResponse> ExecuteAsync(RegisterRequest req, CancellationToken ct)
        => _authService.RegisterAsync(req, HttpContext.Response, ct);
}
