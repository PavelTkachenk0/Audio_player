using Audio_player.DAL;
using Audio_player.DAL.Models;
using Audio_player.Helpers;
using Audio_player.Models.Requests;
using Audio_player.Models.Responses;
using Microsoft.EntityFrameworkCore;
using OtpNet;
using QRCoder;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Audio_player.Services;

public enum ConfirmPasswordResult { UserNotFound, IncorrectPassword, Confirmed }

public enum GenerateQrResult { UserNotFound, TwoFactorDisabled, Generated }

public class AuthService(AppDbContext appDbContext, GenerateTokenService tokenService, IPasswordHasher passwordHasher)
{
    private readonly AppDbContext _appDbContext = appDbContext;
    private readonly GenerateTokenService _tokenService = tokenService;
    private readonly IPasswordHasher _passwordHasher = passwordHasher;

    public async Task<TokenResponse> RegisterAsync(RegisterRequest req, HttpResponse response, CancellationToken ct)
    {
        var user = _appDbContext.AppUsers.Add(new AppUser
        {
            Email = req.Email,
            Password = _passwordHasher.Hash(req.Password),
            UserProfile = new UserProfile
            {
                Birthdate = req.Birthday,
                Name = req.Name,
                Surname = req.Surname
            },
            UserRoles = [new AppUserRole { RoleId = 1 }]
        });

        await _appDbContext.SaveChangesAsync(ct);

        return await IssueTokensAsync(user.Entity.Email, response, ct);
    }

    /// <summary>Returns null on bad credentials; a RequiresTwoFactor response when 2FA is on; otherwise a full token.</summary>
    public async Task<TokenResponse?> LoginAsync(LoginRequest req, HttpResponse response, CancellationToken ct)
    {
        var user = await _appDbContext.AppUsers.SingleOrDefaultAsync(x => x.Email == req.Email, ct);

        if (user == null || !_passwordHasher.Verify(req.Password, user.Password))
        {
            return null;
        }

        if (user.IsTwoFactorEnable)
        {
            return new TokenResponse
            {
                RequiresTwoFactor = true,
                TwoFactorToken = _tokenService.GenerateTwoFactorPendingToken(user.Email)
            };
        }

        return await IssueTokensAsync(user.Email, response, ct);
    }

    /// <summary>Rotates the refresh token. Returns null when it's missing/invalid/expired.</summary>
    public async Task<TokenResponse?> RefreshAsync(string? refreshToken, HttpResponse response, CancellationToken ct)
    {
        if (string.IsNullOrEmpty(refreshToken))
        {
            return null;
        }

        var tokenData = await _appDbContext.RefreshTokens
            .Include(x => x.User)
            .SingleOrDefaultAsync(x => x.Token == refreshToken, ct);

        if (tokenData == null || tokenData.IsRevoked || tokenData.ExpiryDate <= DateTime.UtcNow)
        {
            return null;
        }

        tokenData.IsRevoked = true;
        await _appDbContext.SaveChangesAsync(ct);

        return await IssueTokensAsync(tokenData.User.Email, response, ct);
    }

    public async Task LogoutAsync(string? refreshToken, string? accessToken, HttpResponse response, CancellationToken ct)
    {
        if (!string.IsNullOrEmpty(refreshToken))
        {
            var tokenData = await _appDbContext.RefreshTokens.SingleOrDefaultAsync(x => x.Token == refreshToken, ct);

            if (tokenData != null)
            {
                tokenData.IsRevoked = true;
                await _appDbContext.SaveChangesAsync(ct);
            }
        }

        if (!string.IsNullOrEmpty(accessToken))
        {
            var jti = new JwtSecurityTokenHandler().ReadJwtToken(accessToken)
                .Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Jti)?.Value;

            if (jti != null)
            {
                await _tokenService.RevokeAccessTokenAsync(jti, ct);
            }
        }

        response.Cookies.Delete("refreshToken");
    }

    public async Task<ConfirmPasswordResult> ConfirmPasswordAsync(ClaimsPrincipal principal, string password, CancellationToken ct)
    {
        var user = await FindByClaimAsync(principal, ct);

        if (user == null)
        {
            return ConfirmPasswordResult.UserNotFound;
        }

        return _passwordHasher.Verify(password, user.Password)
            ? ConfirmPasswordResult.Confirmed
            : ConfirmPasswordResult.IncorrectPassword;
    }

    public async Task<bool> SetTwoFactorAsync(ClaimsPrincipal principal, bool enable, CancellationToken ct)
    {
        var user = await FindByClaimAsync(principal, ct);

        if (user == null)
        {
            return false;
        }

        user.IsTwoFactorEnable = enable;
        await _appDbContext.SaveChangesAsync(ct);

        return true;
    }

    public async Task<(GenerateQrResult Status, string? QrCodeImageBase64)> GenerateQrCodeAsync(
        ClaimsPrincipal principal, CancellationToken ct)
    {
        var user = await FindByClaimAsync(principal, ct);

        if (user == null)
        {
            return (GenerateQrResult.UserNotFound, null);
        }

        if (!user.IsTwoFactorEnable)
        {
            return (GenerateQrResult.TwoFactorDisabled, null);
        }

        user.TwoFactorSecret = Base32Encoding.ToString(KeyGeneration.GenerateRandomKey(20));
        await _appDbContext.SaveChangesAsync(ct);

        var otpUri = new OtpUri(OtpType.Totp, user.TwoFactorSecret, user.Email, "Audio_player");

        using var qrGenerator = new QRCodeGenerator();
        using var qrData = qrGenerator.CreateQrCode(otpUri.ToString(), QRCodeGenerator.ECCLevel.Q);
        using var qrCode = new PngByteQRCode(qrData);
        var image = qrCode.GetGraphic(20);

        return (GenerateQrResult.Generated, $"data:image/png;base64,{Convert.ToBase64String(image)}");
    }

    /// <summary>Completes the 2FA login: validates the pending token + TOTP code. Returns null on any failure.</summary>
    public async Task<TokenResponse?> VerifyTwoFactorAsync(Verify2FARequest req, HttpResponse response, CancellationToken ct)
    {
        var email = _tokenService.ValidateTwoFactorPendingToken(req.TwoFactorToken);

        if (email == null)
        {
            return null;
        }

        var user = await _appDbContext.AppUsers.SingleOrDefaultAsync(x => x.Email == email, ct);

        if (user == null || string.IsNullOrEmpty(user.TwoFactorSecret))
        {
            return null;
        }

        var totp = new Totp(Base32Encoding.ToBytes(user.TwoFactorSecret));

        if (!totp.VerifyTotp(req.Code, out long _))
        {
            return null;
        }

        return await IssueTokensAsync(user.Email, response, ct);
    }

    private async Task<TokenResponse> IssueTokensAsync(string email, HttpResponse response, CancellationToken ct)
    {
        var accessToken = await _tokenService.GenerateAccessToken(email, ct);
        await _tokenService.SetRefreshTokenCookieAsync(response, email, ct);

        return new TokenResponse { AccessToken = accessToken };
    }

    private Task<AppUser?> FindByClaimAsync(ClaimsPrincipal principal, CancellationToken ct)
    {
        var email = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        return _appDbContext.AppUsers.SingleOrDefaultAsync(x => x.Email == email, ct);
    }
}
