using Audio_player.DAL;
using Audio_player.Helpers;
using Audio_player.Models.Responses;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;

namespace Audio_player.Endpoints.Authentification;

public class RefreshTokenEndpoint(AppDbContext appDbContext, GenerateTokenHelper tokenHelper) : EndpointWithoutRequest<TokenResponse>
{
    private readonly AppDbContext _appDbContext = appDbContext;
    private readonly GenerateTokenHelper _tokenHelper = tokenHelper;

    public override void Configure()
    {
        Post("refresh");
        Group<AuthentificationGroup>();
        AllowAnonymous();
    }

    public override async Task<TokenResponse> ExecuteAsync(CancellationToken ct)
    {
        var refreshToken = HttpContext.Request.Cookies["refreshToken"];

        if (string.IsNullOrEmpty(refreshToken))
        {
            ThrowError("Refresh token is missing");
        }

        var tokenData = await _appDbContext.RefreshTokens.SingleOrDefaultAsync(x => x.Token == refreshToken, ct);

        if (tokenData == null || tokenData.IsRevoked == true || tokenData.ExpiryDate <= DateTime.UtcNow)
        {
            ThrowError("Invalid or expired refresh token");
        }

        var user = await _appDbContext.RefreshTokens
            .Where(x => x.Token == refreshToken)
            .Select(x => x.User)
            .SingleOrDefaultAsync(ct);

        if (user!.Email == null)
        {
            ThrowError("Invalid refresh token");
        }

        var accessToken = await _tokenHelper.GenerateAccessToken(user.Email, ct);
        await _tokenHelper.SetRefreshTokenCookieAsync(HttpContext.Response, user.Email, ct);

        return new TokenResponse
        {
            AccessToken = accessToken
        };
    }
}
