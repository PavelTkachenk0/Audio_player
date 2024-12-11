using Audio_player.AppSettingsOptions;
using Audio_player.DAL;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Audio_player.Helpers;

public class GenerateTokenService(IOptionsSnapshot<AuthOptions> optionsSnapshot, AppDbContext appDbContext)
{
    private readonly AuthOptions _authOptions = optionsSnapshot.Value;
    private readonly AppDbContext _appDbContext = appDbContext;

    public async Task<string> GenerateAccessToken(string email, CancellationToken ct)
    {
        var jti = Guid.NewGuid().ToString();
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, email),
            new(JwtRegisteredClaimNames.Jti, jti)
        };

        var roles = _appDbContext.AppUsers
            .Where(x => x.Email == email)
            .SelectMany(x => x.Roles.Select(x => x.Name))
            .ToList();

        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var token = new JwtSecurityToken(
        issuer: _authOptions.Issuer,
        audience: _authOptions.Audience,
            claims: claims,
            expires: DateTime.UtcNow.Add(TimeSpan.FromMinutes(15)),
            signingCredentials: new SigningCredentials(_authOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256)
        );

        _appDbContext.AccessTokens.Add(new DAL.Models.AccessToken
        {
            ExpiryDate = DateTime.UtcNow.AddMinutes(15),
            Jti = jti,
            User = await _appDbContext.AppUsers.SingleAsync(x => x.Email == email, ct),
        });

        await _appDbContext.SaveChangesAsync(ct);

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

    public async Task RevokeAccessTokenAsync(string jti, CancellationToken ct)
    {
        var token = await _appDbContext.AccessTokens.SingleOrDefaultAsync(x => x.Jti == jti, ct) 
            ?? throw new Exception("Access token not found");

        token.IsRevoked = true;

        _appDbContext.AccessTokens.Update(token);

        await _appDbContext.SaveChangesAsync(ct);
    }
}
