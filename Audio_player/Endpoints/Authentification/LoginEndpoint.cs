using Audio_player.AppSettingsOptions;
using Audio_player.DAL;
using Audio_player.Helpers;
using Audio_player.Models.Requests;
using Audio_player.Models.Responses;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Audio_player.Endpoints.Authentification;

public class LoginEndpoint(AppDbContext appDbContext, GenerateTokenHelper tokenHelper) : Endpoint<LoginRequest, TokenResponse>
{
    private readonly GenerateTokenHelper _tokenHelper = tokenHelper;
    private readonly AppDbContext _appDbContext = appDbContext;

    public override void Configure()
    {
        Post("login");
        Group<AuthentificationGroup>();
        AllowAnonymous();
    }

    public override async Task<TokenResponse> ExecuteAsync(LoginRequest req, CancellationToken ct)
    {
        var user = await _appDbContext.AppUsers.SingleAsync(x => x.Email == req.Email, ct);

        var accessToken = await _tokenHelper.GenerateAccessToken(user.Email, ct);

        await _tokenHelper.SetRefreshTokenCookieAsync(HttpContext.Response, user.Email, ct);

        return new TokenResponse
        {
            AccessToken = accessToken
        };
    }
}
