using Audio_player.DAL;
using Audio_player.Helpers;
using Audio_player.Models.Requests;
using Audio_player.Models.Responses;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using OtpNet;
using System.Security.Claims;

namespace Audio_player.Endpoints.Authentification;

public class Verify2FAEndpoint(AppDbContext appDbContext, GenerateTokenService tokenService) : Endpoint<Verify2FARequest, TokenResponse?>
{
    private readonly AppDbContext _appDbContext = appDbContext;
    private readonly GenerateTokenService _tokenService = tokenService;

    public override void Configure()
    {
        Post("verify-2fa");
        Group<AuthentificationGroup>();
        AllowAnonymous();
    }

    public override async Task<TokenResponse?> ExecuteAsync(Verify2FARequest req, CancellationToken ct)
    {
        var user = await _appDbContext.AppUsers.Where(x => x.Email == req.Email)
                .SingleOrDefaultAsync(ct);

        if (user == null || string.IsNullOrEmpty(user.TwoFactorSecret))
        {
            await SendUnauthorizedAsync(ct);
            return null;
        }

        var totp = new Totp(Base32Encoding.ToBytes(user.TwoFactorSecret));
        bool isValid = totp.VerifyTotp(req.Code, out long _);

        if (isValid)
        {
            var accessToken = await _tokenService.GenerateAccessToken(user.Email, ct);

            await _tokenService.SetRefreshTokenCookieAsync(HttpContext.Response, user.Email, ct);

            return new TokenResponse
            {
                AccessToken = accessToken
            };
        }
        else
        {
            await SendUnauthorizedAsync(ct);
            return null;
        }
    }
}
