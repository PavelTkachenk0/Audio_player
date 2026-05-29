using Audio_player.DAL;
using Audio_player.Models.DTOs;
using Audio_player.Models.Responses;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Audio_player.Endpoints.Favorites.FavoritePlaylists;

public class GetFavoritePlaylistsEndpoint(AppDbContext appDbContext) : EndpointWithoutRequest<GetFavoritePlaylistsResponse>
{
    private readonly AppDbContext _appDbContext = appDbContext;

    public override void Configure()
    {
        Get("");
        Group<FavoritePlaylistGroup>();
    }

    public override async Task<GetFavoritePlaylistsResponse> ExecuteAsync(CancellationToken ct)
    {
        var email = HttpContext.User.Claims
            .FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value!;
        var userId = await _appDbContext.AppUsers.Where(x => x.Email == email)
                .Select(x => x.UserProfile!.Id)
                .SingleOrDefaultAsync(ct);

        var playlists = await _appDbContext.Playlists
            .Where(x => x.UserPlaylists.Any(up => up.UserId == userId))
            .Select(x => new ShortGenrePlaylistDTO
            {
                CoverPath = x.CoverPath,
                Id = x.Id,
                Name = x.Name
            }).ToListAsync(ct);

        return new GetFavoritePlaylistsResponse
        {
            Result = playlists
        };
    }
}
