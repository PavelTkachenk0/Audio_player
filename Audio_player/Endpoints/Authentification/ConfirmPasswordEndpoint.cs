using Audio_player.Constants;
using Audio_player.DAL;
using Audio_player.Models.Requests;
using Audio_player.Models.Responses;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Audio_player.Endpoints.Authentification;

public class ConfirmPasswordEndpoint(AppDbContext appDbContext) : Endpoint<ConfirmPasswordRequest, ConfirmResponse>
{
    private readonly AppDbContext _appDbContext = appDbContext;

    public override void Configure()
    {
        Post("confirm-password");
        Group<AuthentificationGroup>();
        Policies(PolicyNames.HasAdminOrUserRole);
    }

    public override async Task<ConfirmResponse> ExecuteAsync(ConfirmPasswordRequest req, CancellationToken ct)
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

        if (user.Password != req.Password)
        {
            await SendErrorsAsync(400, ct);
            ThrowError("incorrect_password");
        }

        return new ConfirmResponse
        {
            Success = true
        };
    }
}
