namespace Audio_player.Models.DTOs;

public class TrackDTO
{
    public long Id { get; set; }
    public string SongName { get; set; } = null!;
    public string SongPath { get; set; } = null!;
    public long ListeningCount { get; set; }
    public int Duration { get; set; }
    public bool IsFavorite { get; set; }
    public List<ShortArtistDTO> Artists { get; set; } = [];
    public ShortAlbumDTO Album { get; set; } = null!;
    public List<ShortGenreDTO> Genres { get; set; } = [];
}
