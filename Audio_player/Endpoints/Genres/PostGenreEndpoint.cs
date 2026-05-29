using Audio_player.Constants;
using Audio_player.Models.Requests;
using Audio_player.Services;
using FastEndpoints;

namespace Audio_player.Endpoints.Genres;

public class PostGenreEndpoint(GenreService genreService) : Endpoint<CreateGenreRequest>
{
    private readonly GenreService _genreService = genreService;

    public override void Configure()
    {
        Post("");
        Group<GenreGroup>();
        AllowFileUploads();
        Policies(PolicyNames.HasAdminRole);
    }

    public async override Task HandleAsync(CreateGenreRequest req, CancellationToken ct)
    {
        await _genreService.CreateAsync(req, ct);
        await SendOkAsync(ct);
    }
}
