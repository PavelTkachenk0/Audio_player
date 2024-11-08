namespace Audio_player.Models.DTOs;

public class ShortGenrePlaylistDTO
{
    public long Id { get; set; }
    public string Name { get; set; } = null!;
    public string CoverPath { get; set; } = null!;
}

public class GenrePlaylistDTO : ShortGenrePlaylistDTO
{
    public List<TrackForPlaylistDTO> Songs { get; set; } = [];
}