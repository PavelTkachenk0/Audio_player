using Audio_player.Constants;
using Audio_player.DAL;
using Audio_player.Models.DTOs;
using Audio_player.Models.Requests;
using Audio_player.Models.Responses;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Audio_player.Endpoints.Recommendations;

public class GetRecommendationAlbumsEndpointI(AppDbContext appDbContext) : Endpoint<RecommendationRequest, RecommendationAlbumsResponse>
{
    private readonly AppDbContext _appDbContext = appDbContext;

    public override void Configure()
    {
        Get("albums");
        Group<RecommendationsGroup>();
        Policies(PolicyNames.HasAdminOrUserRole);
    }

    public override async Task<RecommendationAlbumsResponse> ExecuteAsync(RecommendationRequest req, CancellationToken ct)
    {
        var email = HttpContext.User.Claims.
          FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value!;
        var userId = await _appDbContext.AppUsers.Where(x => x.Email == email)
                .Select(x => x.UserProfile!.Id)
                .SingleOrDefaultAsync(ct);
        
        var result = await _appDbContext.Albums
            .Skip((int)(req.Skip == null ? 0 : req.Skip!))
            .Take((int)(req.Take == null ? 0 : req.Take!))
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

        return new RecommendationAlbumsResponse
        {
            Result = result,
            TotalCount = result.Count()
        };
    }
}
