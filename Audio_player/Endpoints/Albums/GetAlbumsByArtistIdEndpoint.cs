using Audio_player.Constants;
using Audio_player.DAL;
using Audio_player.Models.DTOs;
using Audio_player.Models.Responses;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Audio_player.Endpoints.Albums;

public class GetAlbumsByArtistIdEndpoint(AppDbContext appDbContext) : EndpointWithoutRequest<GetAlbumsResponse>
{
    private readonly AppDbContext _appDbContext = appDbContext;
    
    public override void Configure()
    {
        Get("by-artistId/{artistId:int}");
        Group<AlbumGroup>();
        Policies(PolicyNames.HasAdminOrUserRole);
    }

    public override async Task<GetAlbumsResponse> ExecuteAsync(CancellationToken ct)
    {
        var artistId = Route<long>("artistId");

        var email = HttpContext.User.Claims.
          FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value!;
        var userId = await _appDbContext.AppUsers.Where(x => x.Email == email)
                .Select(x => x.UserProfile!.Id)
                .SingleOrDefaultAsync(ct);

        var albums = await _appDbContext.Albums
            .Where(x => x.ArtistsAlbums.Select(x => x.ArtistId).Any(x => x == artistId))
            .Select(x => new AlbumDTO
            {
                AlbumName = x.AlbumName,
                CoverPath = x.CoverPath,
                Genres = x.Genres.Select(x => new ShortGenreDTO
                {
                    Id = x.Id,
                    Name = x.Name
                }).ToList(),
                Artists = x.Artists.Select(x => new ShortArtistDTO
                {
                    ArtistName = x.ArtistName,
                    Id = x.Id,
                }).ToList(),
                Id = x.Id,
                IsFavorite = x.UserAlbums.Any(x => x.UserId == userId),
            }).ToListAsync(ct);

        return new GetAlbumsResponse
        {
            Result = albums
        };
    }
}
