using Audio_player.DAL.Models;
using Audio_player.Models.DTOs;

namespace Audio_player.Mappers;

public static class PlaylistMapper
{
    /// <summary>
    /// Server-side projection Playlist → GenrePlaylistDTO. Single source of truth shared by every
    /// playlist query (genre-playlists, and later favorites/recommendations); kept as an IQueryable
    /// extension so EF translates it to SQL. The userId scopes each song's IsFavorite flag.
    /// </summary>
    public static IQueryable<GenrePlaylistDTO> EntityToDto(this IQueryable<Playlist> playlists, long userId) =>
        playlists.Select(x => new GenrePlaylistDTO
        {
            Id = x.Id,
            CoverPath = x.CoverPath,
            Name = x.Name,
            Songs = x.Songs.Select(s => new TrackForPlaylistDTO
            {
                Id = s.Id,
                SongPath = s.SongPath,
                SongName = s.SongName,
                Duration = s.Duration,
                IsFavorite = s.UserSongs.Any(us => us.UserId == userId),
                CoverPath = s.Album!.CoverPath,
                ListeningCount = s.ListeningCount,
                Artists = s.Artists.Select(x => new ShortArtistDTO
                {
                    ArtistName = x.ArtistName,
                    Id = x.Id,
                }).ToList(),
            }).ToList()
        });

    /// <summary>Trimmed Playlist → ShortGenrePlaylistDTO projection (no songs); shared by the
    /// favorites list, the genre-playlists list and the playlist recommendations.</summary>
    public static IQueryable<ShortGenrePlaylistDTO> EntityToShortDto(this IQueryable<Playlist> playlists) =>
        playlists.Select(x => new ShortGenrePlaylistDTO
        {
            Id = x.Id,
            Name = x.Name,
            CoverPath = x.CoverPath
        });
}
