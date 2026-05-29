using Audio_player.DAL.Models;
using Audio_player.Models.DTOs;

namespace Audio_player.Mappers;

public static class AlbumMapper
{
    /// <summary>
    /// Server-side projection Album → AlbumDTO. Single source of truth shared by every
    /// album query; kept as an IQueryable extension so EF translates it to SQL.
    /// </summary>
    public static IQueryable<AlbumDTO> EntityToDto(this IQueryable<Album> albums, long userId) =>
        albums.Select(x => new AlbumDTO
        {
            AlbumName = x.AlbumName,
            CoverPath = x.CoverPath,
            Genres = x.Genres.Select(g => new ShortGenreDTO { Id = g.Id, Name = g.Name }).ToList(),
            Artists = x.Artists.Select(a => new ShortArtistDTO { ArtistName = a.ArtistName, Id = a.Id }).ToList(),
            Id = x.Id,
            IsFavorite = x.UserAlbums.Any(ua => ua.UserId == userId),
        });
}
