using Audio_player.Constants;
using Audio_player.DAL;
using Audio_player.Models.DTOs;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;

namespace Audio_player.Endpoints.Users;

public class GetUserByIdEndpoint(AppDbContext appDbContext) : EndpointWithoutRequest<UserDTO?>
{
    private readonly AppDbContext _appDbContext = appDbContext;

    public override void Configure()
    {
        Get("{id:int}");
        Group<UserGroup>();
        Policies(PolicyNames.HasAdminRole);
    }

    public override async Task<UserDTO?> ExecuteAsync(CancellationToken ct)
    {
        var id = Route<long>("id");

        var user = await _appDbContext.AppUsers.Select(x => new UserDTO
        {
            Id = x.Id,
            Email = x.Email,
            Birthday = x.UserProfile!.Birthdate,
            Surname = x.UserProfile!.Surname,
            Name = x.UserProfile!.Name,
        }).SingleOrDefaultAsync(x => x.Id == id);

        if (user == null)
        {
            await SendNotFoundAsync(ct);
            return null;
        }

        return user;
    }
}
