namespace Audio_player.Models.DTOs;

public class AlbumDTO
{
    public long Id { get; set; }
    public string AlbumName { get; set; } = null!;
    public string CoverPath { get; set; } = null!;
    public List<ShortGenreDTO> Genres { get; set; } = [];
    public List<ShortArtistDTO> Artists { get; set; } = [];
}
