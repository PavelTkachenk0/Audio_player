using Audio_player.DAL;
using Audio_player.Helpers;
using Audio_player.Models.Requests;
using Audio_player.Models.Responses;
using Audio_player.Services;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;

namespace Audio_player.Endpoints.Authentication;

public class LoginEndpoint(AppDbContext appDbContext, GenerateTokenService tokenService, IPasswordHasher passwordHasher) : Endpoint<LoginRequest, TokenResponse>
{
    private readonly GenerateTokenService _tokenService = tokenService;
    private readonly AppDbContext _appDbContext = appDbContext;
    private readonly IPasswordHasher _passwordHasher = passwordHasher;

    public override void Configure()
    {
        Post("login");
        Group<AuthenticationGroup>();
        AllowAnonymous();
    }

    public override async Task<TokenResponse> ExecuteAsync(LoginRequest req, CancellationToken ct)
    {
        var user = await _appDbContext.AppUsers.SingleOrDefaultAsync(x => x.Email == req.Email, ct);

        if (user == null || !_passwordHasher.Verify(req.Password, user.Password))
        {
            ThrowError("invalid_login_or_password");
        }

        if (user.IsTwoFactorEnable)
        {
            return new TokenResponse
            {
                RequiresTwoFactor = true,
                TwoFactorToken = _tokenService.GenerateTwoFactorPendingToken(user.Email)
            };
        }

        var accessToken = await _tokenService.GenerateAccessToken(user.Email, ct);

        await _tokenService.SetRefreshTokenCookieAsync(HttpContext.Response, user.Email, ct);

        return new TokenResponse
        {
            AccessToken = accessToken
        };
    }
}
