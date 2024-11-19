using Audio_player.Constants;
using Audio_player.DAL;
using Audio_player.Models.DTOs;
using Audio_player.Models.Responses;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;

namespace Audio_player.Endpoints.Users;

public class GetUsersEndpoint(AppDbContext appDbContext) : EndpointWithoutRequest<GetUsersResponse>
{
    private readonly AppDbContext _appDbContext = appDbContext;

    public override void Configure()
    {
        Get("");
        Group<UserGroup>();
        Policies(PolicyNames.HasAdminRole);
    }

    public override async Task<GetUsersResponse> ExecuteAsync(CancellationToken ct)
    {
        var users = await _appDbContext.AppUsers.Select(x => new UserDTO
        {
            Id = x.Id,
            Email = x.Email,
            Birthday = x.UserProfile!.Birthdate,
            Name = x.UserProfile!.Name,
            Surname = x.UserProfile!.Surname
        }).ToListAsync(ct);

        return new GetUsersResponse
        {
            Result = users
        };
    }
}
