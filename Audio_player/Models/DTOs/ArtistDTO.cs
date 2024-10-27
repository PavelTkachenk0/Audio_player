namespace Audio_player.Models.DTOs;

public class ArtistDTO
{
    public long Id { get; set; }
    public string ArtistName { get; set; } = null!;
    public string CoverPath { get; set; } = null!;
    public List<ShortGenreDTO> Genres { get; set; } = [];
}
