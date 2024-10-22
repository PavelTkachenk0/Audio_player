using Audio_player.AppSettingsOptions;
using Audio_player.DAL;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Audio_player.Helpers;

public class GenerateTokenHelper(IOptionsSnapshot<AuthOptions> optionsSnapshot, AppDbContext appDbContext)
{
    private readonly AuthOptions _authOptions = optionsSnapshot.Value;
    private readonly AppDbContext _appDbContext = appDbContext;

    public string GenerateAccessToken(string email)
    {
        var claims = new List<Claim> { new(ClaimTypes.Name, email) };
        var token = new JwtSecurityToken(
        issuer: _authOptions.Issuer,
        audience: _authOptions.Audience,
            claims: claims,
            expires: DateTime.UtcNow.Add(TimeSpan.FromMinutes(15)),
            signingCredentials: new SigningCredentials(_authOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256)
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public async Task<string> GenerateRefreshTokenAsync(string email, CancellationToken ct)
    {
        var refreshToken = Guid.NewGuid().ToString();

        _appDbContext.RefreshTokens.Add(new DAL.Models.RefreshToken
        {
            ExpiryDate = DateTime.UtcNow.AddDays(7),
            Token = refreshToken,
            User = await _appDbContext.AppUsers.SingleAsync(x => x.Email == email, ct)
        });

        await _appDbContext.SaveChangesAsync(ct);

        return refreshToken;
    }

    public async Task<string> SetRefreshTokenCookieAsync(HttpResponse response, string email, CancellationToken ct)
    {
        var refreshToken = await GenerateRefreshTokenAsync(email, ct);

        response.Cookies.Append("refreshToken", refreshToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.UtcNow.AddDays(7)
        });

        return refreshToken;
    }
}
