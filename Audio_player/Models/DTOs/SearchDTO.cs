namespace Audio_player.Models.DTOs;

public class SearchDTO
{
    public long Id { get; set; }
    public string Name { get; set; } = null!;
    public string Source { get; set; } = null!;
    public string CoverPath { get; set; } = null!;
}
