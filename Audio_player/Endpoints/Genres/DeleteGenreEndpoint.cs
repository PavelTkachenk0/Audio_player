using Audio_player.Constants;
using Audio_player.DAL;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;

namespace Audio_player.Endpoints.Genres;

public class DeleteGenreEndpoint(AppDbContext appDbContext) : EndpointWithoutRequest
{
    private readonly AppDbContext _appDbContext = appDbContext;

    public override void Configure()
    {
        Delete("{id:int}");
        Group<GenreGroup>();
        Policies(PolicyNames.HasAdminRole);
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var genreId = Route<short>("id");

        var genre = await _appDbContext.Genres.SingleOrDefaultAsync(x => x.Id == genreId, ct);

        if (genre == null)
        {
            await SendNotFoundAsync(ct);
            return;
        }

        _appDbContext.Genres.Remove(genre);
        await _appDbContext.SaveChangesAsync(ct);

        await SendOkAsync(ct);
    }
}
