using Audio_player.Constants;
using Audio_player.DAL;
using Audio_player.Models.Requests;
using Audio_player.Models.Responses;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Audio_player.Endpoints.Authentification;

public class EnableTwoFactorAthentificationEndpoint(AppDbContext appDbContext) : Endpoint<EnableTwoFactorAthentificationRequest, ConfirmResponse>
{
    private readonly AppDbContext _appDbContext = appDbContext;

    public override void Configure()
    {
        Put("/enable-2fa");
        Group<AuthentificationGroup>();
        Policies(PolicyNames.HasAdminOrUserRole);
    }

    public override async Task<ConfirmResponse> ExecuteAsync(EnableTwoFactorAthentificationRequest req, CancellationToken ct)
    {
        var email = HttpContext.User.Claims.
          FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value!;
        var user = await _appDbContext.AppUsers.Where(x => x.Email == email)
                .SingleOrDefaultAsync(ct);

        if (user == null)
        {
            await SendNotFoundAsync(ct);
            ThrowError("user_is_not_found");
        }

        if (req.Enable)
        {
            user.IsTwoFactorEnable = true;
        }
        else
        {
            user.IsTwoFactorEnable = false;
        }

        _appDbContext.AppUsers.Update(user);

        await _appDbContext.SaveChangesAsync(ct);

        return new ConfirmResponse
        {
            Success = true
        };
    }
}
