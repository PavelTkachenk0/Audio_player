using Audio_player.DAL.Models;
using Audio_player.Models.DTOs;

namespace Audio_player.Mappers;

public static class GenreMapper
{
    /// <summary>
    /// Server-side projection Genre → GenreDTO. Single source of truth shared by every
    /// genre query; kept as an IQueryable extension so EF translates it to SQL.
    /// </summary>
    public static IQueryable<GenreDTO> EntityToDto(this IQueryable<Genre> genres) =>
        genres.Select(x => new GenreDTO
        {
            CoverPath = x.CoverPath,
            Id = x.Id,
            Name = x.Name,
        });
}
