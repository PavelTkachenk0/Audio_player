using Audio_player.DAL;
using Audio_player.Helpers;
using Audio_player.Models.Requests;
using Audio_player.Models.Responses;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;

namespace Audio_player.Endpoints.Authentification;

public class LoginEndpoint(AppDbContext appDbContext, GenerateTokenService tokenService) : Endpoint<LoginRequest, TokenResponse>
{
    private readonly GenerateTokenService _tokenService = tokenService;
    private readonly AppDbContext _appDbContext = appDbContext;

    public override void Configure()
    {
        Post("login");
        Group<AuthentificationGroup>();
        AllowAnonymous();
    }

    public override async Task<TokenResponse> ExecuteAsync(LoginRequest req, CancellationToken ct)
    {
        var user = await _appDbContext.AppUsers.SingleOrDefaultAsync(x => x.Email == req.Email && x.Password == req.Password, ct);

        if (user == null)
        {
            ThrowError("invalid_login_or_password");
            await SendNotFoundAsync(ct);
        }

        var accessToken = await _tokenService.GenerateAccessToken(user.Email, ct);

        await _tokenService.SetRefreshTokenCookieAsync(HttpContext.Response, user.Email, ct);

        return new TokenResponse
        {
            AccessToken = accessToken
        };
    }
}
