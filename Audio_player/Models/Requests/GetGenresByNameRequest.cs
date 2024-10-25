using FastEndpoints;

namespace Audio_player.Models.Requests;

public class GetGenresByNameRequest
{
    [QueryParam]
    public string? Name { get; set; } 
}
