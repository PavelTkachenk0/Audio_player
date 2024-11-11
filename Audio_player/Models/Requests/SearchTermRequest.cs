using FastEndpoints;

namespace Audio_player.Models.Requests;

public class SearchTermRequest
{
    [QueryParam]
    public string? SearchTerm { get; set; }
}
