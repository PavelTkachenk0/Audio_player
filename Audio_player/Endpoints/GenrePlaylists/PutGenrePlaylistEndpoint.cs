using Audio_player.Constants;
using Audio_player.Models.Requests;
using Audio_player.Services;
using FastEndpoints;

namespace Audio_player.Endpoints.GenrePlaylists;

public class PutGenrePlaylistEndpoint(GenrePlaylistService genrePlaylistService) : Endpoint<PutPlaylistRequest>
{
    private readonly GenrePlaylistService _genrePlaylistService = genrePlaylistService;

    public override void Configure()
    {
        Put("{id:int}");
        Group<GenrePlaylistGroup>();
        AllowFileUploads();
        Policies(PolicyNames.HasAdminRole);
    }

    public override async Task HandleAsync(PutPlaylistRequest req, CancellationToken ct)
    {
        var id = Route<long>("id");

        if (!await _genrePlaylistService.UpdateAsync(id, req, ct))
        {
            await SendNotFoundAsync(ct);
            return;
        }

        await SendOkAsync(ct);
    }
}
