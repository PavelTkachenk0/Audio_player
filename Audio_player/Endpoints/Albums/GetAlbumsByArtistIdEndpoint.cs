using Audio_player.Constants;
using Audio_player.Models.Responses;
using Audio_player.Services;
using FastEndpoints;

namespace Audio_player.Endpoints.Albums;

public class GetAlbumsByArtistIdEndpoint(AlbumService albumService) : EndpointWithoutRequest<GetAlbumsResponse>
{
    private readonly AlbumService _albumService = albumService;

    public override void Configure()
    {
        Get("by-artistId/{artistId:int}");
        Group<AlbumGroup>();
        Policies(PolicyNames.HasAdminOrUserRole);
    }

    public override async Task<GetAlbumsResponse> ExecuteAsync(CancellationToken ct)
    {
        var artistId = Route<long>("artistId");

        var albums = await _albumService.GetByArtistIdAsync(artistId, HttpContext.User, ct);

        return new GetAlbumsResponse
        {
            Result = albums
        };
    }
}
