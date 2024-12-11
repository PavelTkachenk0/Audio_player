using Audio_player.Constants;
using Audio_player.DAL;
using Audio_player.Services;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;

namespace Audio_player.Endpoints.Genres;

public class DeleteGenreEndpoint(AppDbContext appDbContext, FileService fileService) : EndpointWithoutRequest
{
    private readonly AppDbContext _appDbContext = appDbContext;
    private readonly FileService _fileService = fileService;

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

        _fileService.DeleteFile(genre.CoverPath);

        await _appDbContext.SaveChangesAsync(ct);

        await SendOkAsync(ct);
    }
}
