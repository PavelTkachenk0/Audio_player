using Audio_player.DAL;
using Audio_player.Helpers;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;

namespace Audio_player.Endpoints.Authentification;

public class LogoutEndpoint(AppDbContext appDbContext, GenerateTokenService tokenService) : EndpointWithoutRequest
{
    private readonly GenerateTokenService _tokenService = tokenService;
    private readonly AppDbContext _appDbContext = appDbContext;

    public override void Configure()
    {
        Post("logout");
        Group<AuthentificationGroup>();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var refreshToken = HttpContext.Request.Cookies["refreshToken"];
        var accessToken = HttpContext.Request.Headers.Authorization.ToString().Replace("Bearer ", "");

        if (!string.IsNullOrEmpty(refreshToken))
        {
            var tokenData = await _appDbContext.RefreshTokens.SingleAsync(x => x.Token == refreshToken, ct);

            tokenData.IsRevoked = true;

            _appDbContext.RefreshTokens.Update(tokenData);
            await _appDbContext.SaveChangesAsync(ct);
        }

        if (!string.IsNullOrEmpty(accessToken))
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(accessToken);
            var jti = jwtToken.Claims.First(x => x.Type == JwtRegisteredClaimNames.Jti).Value;

            await _tokenService.RevokeAccessTokenAsync(jti, ct);
        }

        HttpContext.Response.Cookies.Delete("refreshToken");

        await SendOkAsync(ct);
    }
}
