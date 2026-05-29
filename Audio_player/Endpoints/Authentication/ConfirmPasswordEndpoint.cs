using Audio_player.Constants;
using Audio_player.DAL;
using Audio_player.Models.Requests;
using Audio_player.Models.Responses;
using Audio_player.Services;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Audio_player.Endpoints.Authentication;

public class ConfirmPasswordEndpoint(AppDbContext appDbContext, IPasswordHasher passwordHasher) : Endpoint<ConfirmPasswordRequest, ConfirmResponse>
{
    private readonly AppDbContext _appDbContext = appDbContext;
    private readonly IPasswordHasher _passwordHasher = passwordHasher;

    public override void Configure()
    {
        Post("confirm-password");
        Group<AuthenticationGroup>();
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
            ThrowError("user_is_not_found");
        }

        if (!_passwordHasher.Verify(req.Password, user.Password))
        {
            ThrowError("incorrect_password");
        }

        return new ConfirmResponse
        {
            Success = true
        };
    }
}
