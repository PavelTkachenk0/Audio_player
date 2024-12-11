namespace Audio_player.Models.DTOs;

public class ArtistDTO : ShortArtistDTO
{
    public string AvatarPath { get; set; } = null!;
    public string CoverPath { get; set; } = null!;
    public bool IsFavorite { get; set; }
    public List<ShortGenreDTO> Genres { get; set; } = [];
}

public class ShortArtistDTO
{
    public long Id { get; set; }
    public string ArtistName { get; set; } = null!;
}