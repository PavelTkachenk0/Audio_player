namespace Audio_player.Models.DTOs;

public class GenreDTO : ShortGenreDTO
{
    public string CoverPath { get; set; } = null!;
}

public class ShortGenreDTO
{
    public short Id { get; set; }
    public string Name { get; set; } = null!;
}