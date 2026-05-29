using Audio_player.Constants;
using Audio_player.Models.DTOs;
using Audio_player.Services;
using FastEndpoints;

namespace Audio_player.Endpoints.Albums;

public class GetAlbumByIdEndpoint(AlbumService albumService) : EndpointWithoutRequest<AlbumDTO?>
{
    private readonly AlbumService _albumService = albumService;

    public override void Configure()
    {
        Get("{id:int}");
        Group<AlbumGroup>();
        Policies(PolicyNames.HasAdminOrUserRole);
    }

    public override async Task<AlbumDTO?> ExecuteAsync(CancellationToken ct)
    {
        var id = Route<long>("id");

        var album = await _albumService.GetByIdAsync(id, HttpContext.User, ct);

        if (album == null)
        {
            await SendNotFoundAsync(ct);
            return null;
        }

        return album;
    }
}
