using Audio_player.Constants;
using Audio_player.Services;
using FastEndpoints;

namespace Audio_player.Endpoints.Users;

public class DeleteUserEndpoint(UserService userService) : EndpointWithoutRequest
{
    private readonly UserService _userService = userService;

    public override void Configure()
    {
        Delete("{id:int}");
        Group<UserGroup>();
        Policies(PolicyNames.HasAdminRole);
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var id = Route<long>("id");

        var result = await _userService.DeleteAsync(id, HttpContext.User, ct);

        switch (result)
        {
            case DeleteUserResult.NotFound:
                await SendNotFoundAsync(ct);
                return;
            case DeleteUserResult.SelfDeletion:
                await SendStringAsync("impossible_to_delete_yourself", 400, cancellation: ct);
                return;
            default:
                await SendOkAsync(ct);
                return;
        }
    }
}
