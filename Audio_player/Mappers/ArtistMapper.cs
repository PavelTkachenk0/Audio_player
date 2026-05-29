using Audio_player.DAL.Models;
using Audio_player.Models.DTOs;

namespace Audio_player.Mappers;

public static class ArtistMapper
{
    /// <summary>
    /// Server-side projection Artist → ArtistDTO. Single source of truth shared by every
    /// artist query; kept as an IQueryable extension so EF translates it to SQL.
    /// </summary>
    public static IQueryable<ArtistDTO> EntityToDto(this IQueryable<Artist> artists, long userId) =>
        artists.Select(x => new ArtistDTO
        {
            ArtistName = x.ArtistName,
            CoverPath = x.CoverPath,
            AvatarPath = x.AvatarPath,
            IsFavorite = x.UserArtists.Any(ua => ua.UserId == userId),
            Genres = x.Genres.Select(g => new ShortGenreDTO { Id = g.Id, Name = g.Name }).ToList(),
            Id = x.Id,
        });
}
