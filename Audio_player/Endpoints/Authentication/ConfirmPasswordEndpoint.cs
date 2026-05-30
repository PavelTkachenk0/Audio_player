using Audio_player.Constants;
using Audio_player.Models.Requests;
using Audio_player.Models.Responses;
using Audio_player.Services;
using FastEndpoints;

namespace Audio_player.Endpoints.Authentication;

public class ConfirmPasswordEndpoint(AuthService authService) : Endpoint<ConfirmPasswordRequest, ConfirmResponse>
{
    private readonly AuthService _authService = authService;

    public override void Configure()
    {
        Post("confirm-password");
        Group<AuthenticationGroup>();
        Policies(PolicyNames.HasAdminOrUserRole);
    }

    public override async Task<ConfirmResponse> ExecuteAsync(ConfirmPasswordRequest req, CancellationToken ct)
    {
        var result = await _authService.ConfirmPasswordAsync(HttpContext.User, req.Password, ct);

        if (result == ConfirmPasswordResult.UserNotFound)
        {
            ThrowError("user_is_not_found");
        }

        if (result == ConfirmPasswordResult.IncorrectPassword)
        {
            ThrowError("incorrect_password");
        }

        return new ConfirmResponse { Success = true };
    }
}
