using Audio_player.Constants;
using Audio_player.Models.Requests;
using Audio_player.Models.Responses;
using Audio_player.Services;
using FastEndpoints;

namespace Audio_player.Endpoints.Albums;

public class GetAlbumsByNameEndpoint(AlbumService albumService) : Endpoint<GetByNameRequest, GetAlbumsResponse>
{
    private readonly AlbumService _albumService = albumService;

    public override void Configure()
    {
        Get("");
        Group<AlbumGroup>();
        Policies(PolicyNames.HasAdminOrUserRole);
    }

    public override async Task<GetAlbumsResponse> ExecuteAsync(GetByNameRequest req, CancellationToken ct)
    {
        var albums = await _albumService.GetByNameAsync(req.Name, HttpContext.User, ct);

        return new GetAlbumsResponse
        {
            Result = albums
        };
    }
}
