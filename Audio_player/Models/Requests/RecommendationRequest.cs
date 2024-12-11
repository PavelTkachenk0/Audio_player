using FastEndpoints;

namespace Audio_player.Models.Requests;

public class RecommendationRequest
{
    [QueryParam]
    public int? Take {  get; set; }
    [QueryParam]
    public int? Skip { get; set; }
}
