using Audio_player.DAL.Models;
using Audio_player.Models.DTOs;

namespace Audio_player.Mappers;

public static class TrackMapper
{
    /// <summary>
    /// Server-side projection Song → TrackDTO. Shared source of truth for track detail
    /// queries; kept as an IQueryable extension so EF translates it to SQL. The userId
    /// drives the IsFavorite flag (per-user UserSongs scoping).
    /// </summary>
    public static IQueryable<TrackDTO> EntityToDto(this IQueryable<Song> songs, long userId) =>
        songs.Select(x => new TrackDTO
        {
            Id = x.Id,
            Duration = x.Duration,
            ListeningCount = x.ListeningCount,
            SongName = x.SongName,
            SongPath = x.SongPath,
            IsFavorite = x.UserSongs.Any(us => us.UserId == userId),
            Album = new ShortAlbumDTO
            {
                AlbumName = x.Album!.AlbumName,
                CoverPath = x.Album!.CoverPath,
                Id = x.Album!.Id
            },
            Genres = x.Genres.Select(g => new ShortGenreDTO
            {
                Id = g.Id,
                Name = g.Name
            }).ToList(),
            Artists = x.Artists.Select(a => new ShortArtistDTO
            {
                ArtistName = a.ArtistName,
                Id = a.Id
            }).ToList()
        });
}
