using Audio_player.Constants;
using Audio_player.Models.Requests;
using Audio_player.Models.Responses;
using Audio_player.Services;
using FastEndpoints;

namespace Audio_player.Endpoints.Search;

public class SearchEndpoint(SearchService searchService) : Endpoint<SearchTermRequest, SearchResponse>
{
    private readonly SearchService _searchService = searchService;

    public override void Configure()
    {
        Get("");
        Group<SearchGroup>();
        Policies(PolicyNames.HasAdminOrUserRole);
    }

    public override async Task<SearchResponse> ExecuteAsync(SearchTermRequest req, CancellationToken ct)
    {
        var searchResult = await _searchService.SearchAsync(req.SearchTerm, ct);

        return new SearchResponse
        {
            Result = searchResult
        };
    }
}
