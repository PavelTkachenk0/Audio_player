using Audio_player.Constants;
using Audio_player.DAL;
using Audio_player.Models.DTOs;
using Audio_player.Models.Requests;
using Audio_player.Models.Responses;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Audio_player.Endpoints.Recommendations;

public class GetRecommendationPlaylistsEndpoint(AppDbContext appDbContext) : Endpoint<RecommendationRequest, GetRecommendationPlaylistsResposne>
{
    private readonly AppDbContext _appDbContext = appDbContext;

    public override void Configure()
    {
        Get("playlists");
        Group<RecommendationsGroup>();
        Policies(PolicyNames.HasAdminOrUserRole);
    }

    public override async Task<GetRecommendationPlaylistsResposne> ExecuteAsync(RecommendationRequest req, CancellationToken ct)
    {
        var email = HttpContext.User.Claims.
         FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value!;
        var userId = await _appDbContext.AppUsers.Where(x => x.Email == email)
        .Select(x => x.UserProfile!.Id)
                .SingleOrDefaultAsync(ct);

        var query = _appDbContext.Playlists
            .Where(x => x.IsAdmin);

        var totalCount = await query.CountAsync(ct);

        var playlists = await query
            .OrderBy(x => x.Id)
            .Skip((int)(req.Skip == null ? 0 : req.Skip!))
            .Take((int)(req.Take == null ? 10 : req.Take!))
            .Select(x => new ShortGenrePlaylistDTO
            {
                Id = x.Id,
                CoverPath = x.CoverPath,
                Name = x.Name,
            }).ToListAsync(ct);

        return new GetRecommendationPlaylistsResposne
        {
            Result = playlists,
            TotalCount = totalCount
        };
    }
}
