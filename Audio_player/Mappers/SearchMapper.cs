using Audio_player.DAL.Models;
using Audio_player.Models.DTOs;

namespace Audio_player.Mappers;

public static class SearchMapper
{
    /// <summary>
    /// Server-side projection Album → SearchDTO (Source = nameof(Album)).
    /// Kept as an IQueryable extension so EF translates it to SQL and it can be Union-ed
    /// with the other search sources before a single materialization.
    /// </summary>
    public static IQueryable<SearchDTO> ToSearchDto(this IQueryable<Album> albums) =>
        albums.Select(al => new SearchDTO
        {
            Id = al.Id,
            Name = al.AlbumName,
            Source = nameof(Album),
            CoverPath = al.CoverPath
        });

    /// <summary>
    /// Server-side projection Song → SearchDTO (Source = nameof(Song)).
    /// </summary>
    public static IQueryable<SearchDTO> ToSearchDto(this IQueryable<Song> songs) =>
        songs.Select(s => new SearchDTO
        {
            Id = s.Id,
            Name = s.SongName,
            Source = nameof(Song),
            CoverPath = s.Album!.CoverPath
        });

    /// <summary>
    /// Server-side projection Artist → SearchDTO (Source = nameof(Artist)).
    /// </summary>
    public static IQueryable<SearchDTO> ToSearchDto(this IQueryable<Artist> artists) =>
        artists.Select(ar => new SearchDTO
        {
            Id = ar.Id,
            Name = ar.ArtistName,
            Source = nameof(Artist),
            CoverPath = ar.AvatarPath
        });
}
