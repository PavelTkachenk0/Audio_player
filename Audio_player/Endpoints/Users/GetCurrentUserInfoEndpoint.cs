using Audio_player.Constants;
using Audio_player.DAL;
using Audio_player.Models.DTOs;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Audio_player.Endpoints.Users;

public class GetCurrentUserInfoEndpoint(AppDbContext appDbContext) : EndpointWithoutRequest<UserDTO>
{
    private readonly AppDbContext _appDbContext = appDbContext;

    public override void Configure()
    {
        Get("me");
        Group<UserGroup>();
        Policies(PolicyNames.HasAdminOrUserRole);
    }

    public override async Task<UserDTO> ExecuteAsync(CancellationToken ct)
    {
        var email = HttpContext.User.Claims.
         FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value!;

        var user = await _appDbContext.AppUsers.Select(x => new UserDTO
        {
            Email = x.Email,
            Id = x.Id,
            Birthday = x.UserProfile!.Birthdate,
            Name = x.UserProfile!.Name,
            Surname = x.UserProfile!.Surname,
        }).SingleAsync(x => x.Email == email, ct);

        return user;
    }
}
