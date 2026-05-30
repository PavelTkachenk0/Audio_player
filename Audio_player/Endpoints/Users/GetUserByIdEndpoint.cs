using Audio_player.Constants;
using Audio_player.Models.DTOs;
using Audio_player.Services;
using FastEndpoints;

namespace Audio_player.Endpoints.Users;

public class GetUserByIdEndpoint(UserService userService) : EndpointWithoutRequest<UserDTO?>
{
    private readonly UserService _userService = userService;

    public override void Configure()
    {
        Get("{id:int}");
        Group<UserGroup>();
        Policies(PolicyNames.HasAdminRole);
    }

    public override async Task<UserDTO?> ExecuteAsync(CancellationToken ct)
    {
        var id = Route<long>("id");

        var user = await _userService.GetByIdAsync(id, ct);

        if (user == null)
        {
            await SendNotFoundAsync(ct);
            return null;
        }

        return user;
    }
}
