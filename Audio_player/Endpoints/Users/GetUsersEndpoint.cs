using Audio_player.Constants;
using Audio_player.Models.Responses;
using Audio_player.Services;
using FastEndpoints;

namespace Audio_player.Endpoints.Users;

public class GetUsersEndpoint(UserService userService) : EndpointWithoutRequest<GetUsersResponse>
{
    private readonly UserService _userService = userService;

    public override void Configure()
    {
        Get("");
        Group<UserGroup>();
        Policies(PolicyNames.HasAdminRole);
    }

    public override async Task<GetUsersResponse> ExecuteAsync(CancellationToken ct)
    {
        var users = await _userService.GetAllAsync(ct);

        return new GetUsersResponse
        {
            Result = users
        };
    }
}
