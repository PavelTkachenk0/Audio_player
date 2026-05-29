using Audio_player.Constants;
using Audio_player.DAL;
using Audio_player.Models.DTOs;
using Audio_player.Models.Responses;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;

namespace Audio_player.Endpoints.Genres;

public class GetGenreByIdEndpoint(AppDbContext appDbContext) : EndpointWithoutRequest<GenreDTO?>
{
    private readonly AppDbContext _appDbContext = appDbContext;

    public override void Configure()
    {
        Get("{id:int}");
        Group<GenreGroup>();
        Policies(PolicyNames.HasAdminOrUserRole);
    }

    public override async Task<GenreDTO?> ExecuteAsync(CancellationToken ct)
    {
        var genreId = Route<short>("id");

        var genre = await _appDbContext.Genres.Where(x => x.Id == genreId).Select(x => new GenreDTO
        {
            CoverPath = x.CoverPath,
            Id = x.Id,
            Name = x.Name,
        }).SingleOrDefaultAsync(ct);

        if (genre == null)
        {
            await SendNotFoundAsync(ct);
            return null;
        }

        return genre;
    }
}
