namespace Audio_player.Models.Requests;

public class RegisterRequest
{
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string? Name { get; set; } 
    public string? Surname { get; set; }
    public DateOnly? Birthday { get; set; }
}
