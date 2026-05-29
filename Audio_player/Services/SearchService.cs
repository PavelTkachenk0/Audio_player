using Audio_player.DAL;
using Audio_player.Mappers;
using Audio_player.Models.DTOs;
using Microsoft.EntityFrameworkCore;

namespace Audio_player.Services;

public class SearchService(AppDbContext appDbContext)
{
    private readonly AppDbContext _appDbContext = appDbContext;

    public async Task<List<SearchDTO>> SearchAsync(string? term, CancellationToken ct)
    {
        var albums = _appDbContext.Albums
            .Where(al => EF.Functions.ILike(al.AlbumName, $"%{term}%"))
            .ToSearchDto();

        var songs = _appDbContext.Songs
            .Where(s => EF.Functions.ILike(s.SongName, $"%{term}%"))
            .ToSearchDto();

        var artists = _appDbContext.Artists
            .Where(x => EF.Functions.ILike(x.ArtistName, $"%{term}%"))
            .ToSearchDto();

        return await albums
            .Union(songs)
            .Union(artists)
            .ToListAsync(ct);
    }
}
