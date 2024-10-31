using FastEndpoints;

namespace Audio_player.Models.Requests;

public class GetByNameRequest
{
    [QueryParam]
    public string? Name { get; set; }
}
