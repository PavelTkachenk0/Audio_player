namespace Audio_player.Models.DTOs;

public class GenrePlaylistDTO
{
    public long Id { get; set; }
    public string Name { get; set; } = null!;
    public string CoverPath { get; set; } = null!;
}
