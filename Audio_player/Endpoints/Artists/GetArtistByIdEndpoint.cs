using Audio_player.Constants;
using Audio_player.DAL;
using Audio_player.Models.DTOs;
using Audio_player.Models.Responses;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;

namespace Audio_player.Endpoints.Artists;

public class GetArtistByIdEndpoint(AppDbContext appDbContext) : EndpointWithoutRequest<ArtistDTO?>
{
    private readonly AppDbContext _appDbContext = appDbContext;

    public override void Configure()
    {
        Get("{id:int}");
        Group<ArtistGroup>();
        Policies(PolicyNames.HasAdminOrUserRole);
    }

    public override async Task<ArtistDTO?> ExecuteAsync(CancellationToken ct)
    {
        var id = Route<long>("id");

        var artist = await _appDbContext.Artists.Select(x => new ArtistDTO
        {
            ArtistName = x.ArtistName,
            CoverPath = x.CoverPath,
            Id = x.Id,
            Genres = x.Genres.Select(x => new ShortGenreDTO
            {
                Id = x.Id,
                Name = x.Name
            }).ToList()
        }).SingleOrDefaultAsync(x => x.Id == id, ct);

        if (artist == null)
        {
            await SendNotFoundAsync(ct);
            return null;
        }

        return artist;
    }
}
