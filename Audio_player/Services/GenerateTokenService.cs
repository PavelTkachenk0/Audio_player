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

    private const int AccessTokenMinutes = 15;
    private const int RefreshTokenDays = 7;
    private const int TwoFactorPendingMinutes = 5;
    private const string TokenUseClaim = "token_use";
    private const string TwoFactorPendingTokenUse = "2fa_pending";

    public async Task<string> GenerateAccessToken(string email, CancellationToken ct)
    {
        var jti = Guid.NewGuid().ToString();

        var user = await _appDbContext.AppUsers
            .Where(x => x.Email == email)
            .Select(x => new { x.Id, Roles = x.Roles.Select(r => r.Name).ToList() })
            .SingleAsync(ct);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, email),
            new(JwtRegisteredClaimNames.Jti, jti)
        };

        claims.AddRange(user.Roles.Select(role => new Claim(ClaimTypes.Role, role)));

        var token = new JwtSecurityToken(
            issuer: _authOptions.Issuer,
            audience: _authOptions.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(AccessTokenMinutes),
            signingCredentials: new SigningCredentials(_authOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256)
        );

        _appDbContext.AccessTokens.Add(new DAL.Models.AccessToken
        {
            ExpiryDate = DateTime.UtcNow.AddMinutes(AccessTokenMinutes),
            Jti = jti,
            UserId = user.Id
        });

        await _appDbContext.SaveChangesAsync(ct);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    /// <summary>
    /// Short-lived token issued after a correct password when 2FA is enabled.
    /// It only proves "password step passed"; it carries no roles, so it cannot
    /// access protected endpoints — it is exchanged at /verify-2fa for a real token.
    /// </summary>
    public string GenerateTwoFactorPendingToken(string email)
    {
        var token = new JwtSecurityToken(
            issuer: _authOptions.Issuer,
            audience: _authOptions.Audience,
            claims: new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, email),
                new Claim(TokenUseClaim, TwoFactorPendingTokenUse)
            },
            expires: DateTime.UtcNow.AddMinutes(TwoFactorPendingMinutes),
            signingCredentials: new SigningCredentials(_authOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256)
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    /// <summary>Returns the email if the pending 2FA token is valid, otherwise null.</summary>
    public string? ValidateTwoFactorPendingToken(string token)
    {
        try
        {
            var principal = new JwtSecurityTokenHandler().ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = _authOptions.Issuer,
                ValidateAudience = true,
                ValidAudience = _authOptions.Audience,
                ValidateLifetime = true,
                IssuerSigningKey = _authOptions.GetSymmetricSecurityKey(),
                ValidateIssuerSigningKey = true,
                ClockSkew = TimeSpan.FromSeconds(30)
            }, out _);

            if (principal.FindFirst(TokenUseClaim)?.Value != TwoFactorPendingTokenUse)
            {
                return null;
            }

            return principal.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
                ?? principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }
        catch
        {
            return null;
        }
    }

    public async Task<string> GenerateRefreshTokenAsync(string email, CancellationToken ct)
    {
        var refreshToken = Guid.NewGuid().ToString();

        var userId = await _appDbContext.AppUsers
            .Where(x => x.Email == email)
            .Select(x => x.Id)
            .SingleAsync(ct);

        _appDbContext.RefreshTokens.Add(new DAL.Models.RefreshToken
        {
            ExpiryDate = DateTime.UtcNow.AddDays(RefreshTokenDays),
            Token = refreshToken,
            UserId = userId
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
            Expires = DateTime.UtcNow.AddDays(RefreshTokenDays)
        });

        return refreshToken;
    }

    public async Task RevokeAccessTokenAsync(string jti, CancellationToken ct)
    {
        var token = await _appDbContext.AccessTokens.SingleOrDefaultAsync(x => x.Jti == jti, ct);

        if (token == null)
        {
            return;
        }

        token.IsRevoked = true;

        await _appDbContext.SaveChangesAsync(ct);
    }
}
