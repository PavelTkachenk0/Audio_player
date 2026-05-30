using Audio_player.Constants;
using Audio_player.Models.DTOs;
using Audio_player.Services;
using FastEndpoints;

namespace Audio_player.Endpoints.Genres;

public class GetGenreByIdEndpoint(GenreService genreService) : EndpointWithoutRequest<GenreDTO?>
{
    private readonly GenreService _genreService = genreService;

    public override void Configure()
    {
        Get("{id:int}");
        Group<GenreGroup>();
        Policies(PolicyNames.HasAdminOrUserRole);
    }

    public override async Task<GenreDTO?> ExecuteAsync(CancellationToken ct)
    {
        var genreId = Route<short>("id");

        var genre = await _genreService.GetByIdAsync(genreId, ct);

        if (genre == null)
        {
            await SendNotFoundAsync(ct);
            return null;
        }

        return genre;
    }
}
