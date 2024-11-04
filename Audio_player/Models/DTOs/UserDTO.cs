namespace Audio_player.Models.DTOs;

public class UserDTO
{
    public long Id { get; set; }
    public string? Name { get; set; }
    public string? Surname { get; set; }
    public DateOnly? Birthday { get; set; }
    public string Email { get; set; } = null!;
}
