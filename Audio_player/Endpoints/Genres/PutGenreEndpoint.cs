using Audio_player.Constants;
using Audio_player.Models.Requests;
using Audio_player.Services;
using FastEndpoints;

namespace Audio_player.Endpoints.Genres;

public class PutGenreEndpoint(GenreService genreService) : Endpoint<EditGenreRequest>
{
    private readonly GenreService _genreService = genreService;

    public override void Configure()
    {
        Put("{id:int}");
        Group<GenreGroup>();
        AllowFileUploads();
        Policies(PolicyNames.HasAdminRole);
    }

    public override async Task HandleAsync(EditGenreRequest req, CancellationToken ct)
    {
        var id = Route<short>("id");

        if (!await _genreService.UpdateAsync(id, req, ct))
        {
            await SendNotFoundAsync(ct);
            return;
        }

        await SendOkAsync(ct);
    }
}
