using Audio_player.Constants;
using Audio_player.DAL;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Audio_player.Endpoints.Users;

public class DeleteUserEndpoint(AppDbContext appDbContext) : EndpointWithoutRequest
{
    private readonly AppDbContext _appDbContext = appDbContext;

    public override void Configure()
    {
        Delete("{id:int}");
        Group<UserGroup>();
        Policies(PolicyNames.HasAdminRole);
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var id = Route<long>("id");

        var user = await _appDbContext.AppUsers.SingleOrDefaultAsync(x => x.Id == id, ct);

        if (user == null)
        {
            await SendNotFoundAsync(ct);
            return;
        }

        if (user.Email == HttpContext.User.Claims.
            FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value!)
        {
            await SendStringAsync("impossible_to_delete_yourself", 400, cancellation: ct);
            return;
        }

        _appDbContext.AppUsers.Remove(user);
        await _appDbContext.SaveChangesAsync(ct);

        await SendOkAsync(ct);
    }
}
