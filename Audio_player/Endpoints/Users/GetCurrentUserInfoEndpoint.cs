using Audio_player.Constants;
using Audio_player.Models.DTOs;
using Audio_player.Services;
using FastEndpoints;

namespace Audio_player.Endpoints.Users;

public class GetCurrentUserInfoEndpoint(UserService userService) : EndpointWithoutRequest<UserDTO>
{
    private readonly UserService _userService = userService;

    public override void Configure()
    {
        Get("me");
        Group<UserGroup>();
        Policies(PolicyNames.HasAdminOrUserRole);
    }

    public override async Task<UserDTO> ExecuteAsync(CancellationToken ct)
    {
        return await _userService.GetCurrentAsync(HttpContext.User, ct);
    }
}
