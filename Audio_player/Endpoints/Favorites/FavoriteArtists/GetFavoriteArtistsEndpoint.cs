using Audio_player.DAL;
using Audio_player.Models.DTOs;
using Audio_player.Models.Responses;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Audio_player.Endpoints.Favorites.FavoriteArtists;

public class GetFavoriteArtistsEndpoint(AppDbContext appDbContext) : EndpointWithoutRequest<GetFavoriteArtistsResponse>
{
    private readonly AppDbContext _appDbContext = appDbContext;

    public override void Configure()
    {
        Get("");
        Group<FavoriteArtistsGroup>();
    }

    public override async Task<GetFavoriteArtistsResponse> ExecuteAsync(CancellationToken ct)
    {
        var email = HttpContext.User.Claims.
          FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value!;
        var userId = await _appDbContext.AppUsers.Where(x => x.Email == email)
                .Select(x => x.UserProfile!.Id)
                .SingleOrDefaultAsync(ct);

        var artists = await _appDbContext.Artists
            .Where(x => x.UserArtists.Select(us => us.ArtistId).Contains(x.Id))
            .Select(x => new ArtistDTO
            {
                ArtistName = x.ArtistName,
                CoverPath = x.CoverPath,
                AvatarPath = x.AvatarPath,
                IsFavorite = x.UserArtists.Any(x => x.UserId == userId),
                Genres = x.Genres.Select(x => new ShortGenreDTO
                {
                    Id = x.Id,
                    Name = x.Name
                }).ToList(),
                Id = x.Id
            })
            .ToListAsync(ct);

        return new GetFavoriteArtistsResponse
        {
            Result = artists
        };
    }
}
