namespace Audio_player.Models.DTOs;

public class GenreDTO
{
    public short Id { get; set; }
    public string Name { get; set; } = null!;
    public string CoverPath { get; set; } = null!;
}
