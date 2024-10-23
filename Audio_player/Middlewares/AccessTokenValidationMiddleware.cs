using Audio_player.DAL;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;

namespace Audio_player.Middlewares;

public class AccessTokenValidationMiddleware(RequestDelegate requestDelegate)
{
    private readonly RequestDelegate _requestDelegate = requestDelegate;

    public async Task InvokeAsync(HttpContext httpContext)
    {
        var accessToken = httpContext.Request.Headers.Authorization.ToString().Replace("Bearer ", "");
        CancellationToken ct = new();

        if (!string.IsNullOrEmpty(accessToken))
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(accessToken);
            var jti = jwtToken.Claims.First(x => x.Type == JwtRegisteredClaimNames.Jti).Value;

            var dbContext = httpContext.RequestServices.GetRequiredService<AppDbContext>();

            var isRevoke = await dbContext.AccessTokens.Where(x => x.Jti == jti).Select(x => x.IsRevoked).SingleAsync(ct);

            if (isRevoke)
            {
                httpContext.Response.StatusCode = 401;
                await httpContext.Response.WriteAsync("Access token has been revoked");
                return;
            }
        }

        await _requestDelegate(httpContext);
    }
}
