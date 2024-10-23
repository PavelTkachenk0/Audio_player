using Audio_player.AppSettingsOptions;
using Audio_player.DAL;
using Audio_player.Helpers;
using Audio_player.Models.Requests;
using Audio_player.Models.Responses;
using FastEndpoints;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Audio_player.Endpoints.Authentification;

public class RegisterEndpoint(AppDbContext appDbContext, GenerateTokenHelper tokenHelper) : Endpoint<RegisterRequest, TokenResponse>
{
    private readonly GenerateTokenHelper _tokenHelper = tokenHelper;
    private readonly AppDbContext _appDbContext = appDbContext;

    public override void Configure()
    {
        Post("register");
        Group<AuthentificationGroup>();
        AllowAnonymous();
    }

    public override async Task<TokenResponse> ExecuteAsync(RegisterRequest req, CancellationToken ct)
    {
        var user = _appDbContext.AppUsers.Add(new DAL.Models.AppUser
        {
            Email = req.Email,
            Password = req.Password,
            UserProfile = new DAL.Models.UserProfile
            {
                Birthdate = req.Birthday,
                Name = req.Name,
                Surname = req.Surname
            }
        });

        await _appDbContext.SaveChangesAsync(ct);

        var accessToken = await _tokenHelper.GenerateAccessToken(user.Entity.Email, ct);

        await _tokenHelper.SetRefreshTokenCookieAsync(HttpContext.Response, user.Entity.Email, ct);

        return new TokenResponse
        {
            AccessToken = accessToken
        };
    }
}
