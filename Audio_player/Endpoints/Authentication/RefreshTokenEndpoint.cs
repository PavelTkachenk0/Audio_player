using Audio_player.DAL;
using Audio_player.Helpers;
using Audio_player.Models.Responses;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;

namespace Audio_player.Endpoints.Authentication;

public class RefreshTokenEndpoint(AppDbContext appDbContext, GenerateTokenService tokenService) : EndpointWithoutRequest<TokenResponse>
{
    private readonly AppDbContext _appDbContext = appDbContext;
    private readonly GenerateTokenService _tokenService = tokenService;

    public override void Configure()
    {
        Post("refresh");
        Group<AuthenticationGroup>();
        AllowAnonymous();
    }

    public override async Task<TokenResponse> ExecuteAsync(CancellationToken ct)
    {
        var refreshToken = HttpContext.Request.Cookies["refreshToken"];

        if (string.IsNullOrEmpty(refreshToken))
        {
            ThrowError("Refresh token is missing");
        }

        var tokenData = await _appDbContext.RefreshTokens
            .Include(x => x.User)
            .SingleOrDefaultAsync(x => x.Token == refreshToken, ct);

        if (tokenData == null || tokenData.IsRevoked || tokenData.ExpiryDate <= DateTime.UtcNow)
        {
            ThrowError("Invalid or expired refresh token");
        }

        var email = tokenData.User.Email;

        // Rotate the refresh token: revoke the one just used and issue a fresh one.
        tokenData.IsRevoked = true;
        await _appDbContext.SaveChangesAsync(ct);

        var accessToken = await _tokenService.GenerateAccessToken(email, ct);
        await _tokenService.SetRefreshTokenCookieAsync(HttpContext.Response, email, ct);

        return new TokenResponse
        {
            AccessToken = accessToken
        };
    }
}
