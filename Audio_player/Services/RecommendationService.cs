using Audio_player.DAL;
using Audio_player.Extensions;
using Audio_player.Mappers;
using Audio_player.Models.DTOs;
using Audio_player.Models.Requests;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Audio_player.Services;

public class RecommendationService(AppDbContext appDbContext)
{
    private readonly AppDbContext _appDbContext = appDbContext;

    public async Task<(List<TrackDTO> Items, long TotalCount)> GetTracksAsync(
        RecommendationRequest req, ClaimsPrincipal user, CancellationToken ct)
    {
        var userId = await _appDbContext.GetCurrentUserProfileIdAsync(user, ct);

        var query = _appDbContext.Songs;

        var totalCount = await query.CountAsync(ct);

        var items = await query
            .OrderByDescending(x => x.ListeningCount)
            .Skip(req.Skip ?? 0)
            .Take(req.Take ?? 10)
            .EntityToDto(userId)
            .ToListAsync(ct);

        return (items, totalCount);
    }

    public async Task<(List<ArtistDTO> Items, long TotalCount)> GetArtistsAsync(
        RecommendationRequest req, ClaimsPrincipal user, CancellationToken ct)
    {
        var userId = await _appDbContext.GetCurrentUserProfileIdAsync(user, ct);

        var query = _appDbContext.Artists;

        var totalCount = await query.CountAsync(ct);

        var items = await query
            .OrderBy(x => x.Id)
            .Skip(req.Skip ?? 0)
            .Take(req.Take ?? 10)
            .EntityToDto(userId)
            .ToListAsync(ct);

        return (items, totalCount);
    }

    public async Task<(List<AlbumDTO> Items, long TotalCount)> GetAlbumsAsync(
        RecommendationRequest req, ClaimsPrincipal user, CancellationToken ct)
    {
        var userId = await _appDbContext.GetCurrentUserProfileIdAsync(user, ct);

        var query = _appDbContext.Albums;

        var totalCount = await query.CountAsync(ct);

        var items = await query
            .OrderBy(x => x.Id)
            .Skip(req.Skip ?? 0)
            .Take(req.Take ?? 10)
            .EntityToDto(userId)
            .ToListAsync(ct);

        return (items, totalCount);
    }

    public async Task<(List<ShortGenrePlaylistDTO> Items, long TotalCount)> GetPlaylistsAsync(
        RecommendationRequest req, ClaimsPrincipal user, CancellationToken ct)
    {
        var query = _appDbContext.Playlists
            .Where(x => x.IsAdmin);

        var totalCount = await query.CountAsync(ct);

        var items = await query
            .OrderBy(x => x.Id)
            .Skip(req.Skip ?? 0)
            .Take(req.Take ?? 10)
            .EntityToShortDto()
            .ToListAsync(ct);

        return (items, totalCount);
    }
}
