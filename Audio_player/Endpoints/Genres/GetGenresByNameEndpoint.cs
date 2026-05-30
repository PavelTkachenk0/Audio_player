using Audio_player.Constants;
using Audio_player.Models.Requests;
using Audio_player.Models.Responses;
using Audio_player.Services;
using FastEndpoints;

namespace Audio_player.Endpoints.Genres;

public class GetGenresByNameEndpoint(GenreService genreService) : Endpoint<GetByNameRequest, GetGenresResponse>
{
    private readonly GenreService _genreService = genreService;

    public override void Configure()
    {
        Get("");
        Group<GenreGroup>();
        Policies(PolicyNames.HasAdminOrUserRole);
    }

    public override async Task<GetGenresResponse> ExecuteAsync(GetByNameRequest req, CancellationToken ct)
    {
        var genres = await _genreService.GetByNameAsync(req.Name, ct);

        return new GetGenresResponse
        {
            Result = genres
        };
    }
}
