using Audio_player.DAL;
using Audio_player.Models.DTOs;
using Audio_player.Models.Responses;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;

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
        var playlists = await _appDbContext.Playlists.Where(x => x.IsAdmin).Select(x => new ShortGenrePlaylistDTO
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
