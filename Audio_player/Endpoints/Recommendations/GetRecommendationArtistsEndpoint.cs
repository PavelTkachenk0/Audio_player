using Audio_player.Constants;
using Audio_player.DAL;
using Audio_player.Models.DTOs;
using Audio_player.Models.Requests;
using Audio_player.Models.Responses;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Audio_player.Endpoints.Recommendations;

public class GetRecommendationArtistsEndpoint(AppDbContext appDbContext) : Endpoint<RecommendationRequest, GetRecommendationArtistsResponse>
{
    private readonly AppDbContext _appDbContext = appDbContext;

    public override void Configure()
    {
        Get("artists");
        Group<RecommendationsGroup>();
        Policies(PolicyNames.HasAdminOrUserRole);
    }

    public override async Task<GetRecommendationArtistsResponse> ExecuteAsync(RecommendationRequest req, CancellationToken ct)
    {
        var email = HttpContext.User.Claims.
           FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value!;
        var userId = await _appDbContext.AppUsers.Where(x => x.Email == email)
                .Select(x => x.UserProfile!.Id)
                .SingleOrDefaultAsync(ct);

        var artists = await _appDbContext.Artists
            .Skip((int)(req.Skip == null ? 0 : req.Skip!))
            .Take((int)(req.Take == null ? 0 : req.Take!))
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

        return new GetRecommendationArtistsResponse
        {
            Result = artists,
            TotalCount = artists.Count
        };
    }
}
