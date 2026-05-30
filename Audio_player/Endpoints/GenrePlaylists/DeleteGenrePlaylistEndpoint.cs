using Audio_player.Constants;
using Audio_player.Services;
using FastEndpoints;

namespace Audio_player.Endpoints.GenrePlaylists;

public class DeleteGenrePlaylistEndpoint(GenrePlaylistService genrePlaylistService) : EndpointWithoutRequest
{
    private readonly GenrePlaylistService _genrePlaylistService = genrePlaylistService;

    public override void Configure()
    {
        Delete("{id:int}");
        Group<GenrePlaylistGroup>();
        Policies(PolicyNames.HasAdminRole);
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var id = Route<long>("id");

        if (!await _genrePlaylistService.DeleteAsync(id, ct))
        {
            await SendNotFoundAsync(ct);
            return;
        }

        await SendOkAsync(ct);
    }
}
