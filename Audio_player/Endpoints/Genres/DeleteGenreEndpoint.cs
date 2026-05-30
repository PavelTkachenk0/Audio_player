using Audio_player.Constants;
using Audio_player.Services;
using FastEndpoints;

namespace Audio_player.Endpoints.Genres;

public class DeleteGenreEndpoint(GenreService genreService) : EndpointWithoutRequest
{
    private readonly GenreService _genreService = genreService;

    public override void Configure()
    {
        Delete("{id:int}");
        Group<GenreGroup>();
        Policies(PolicyNames.HasAdminRole);
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var genreId = Route<short>("id");

        if (!await _genreService.DeleteAsync(genreId, ct))
        {
            await SendNotFoundAsync(ct);
            return;
        }

        await SendOkAsync(ct);
    }
}
