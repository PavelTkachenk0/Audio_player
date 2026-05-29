using Audio_player.Constants;
using Audio_player.Models.Responses;
using Audio_player.Services;
using FastEndpoints;

namespace Audio_player.Endpoints.GenrePlaylists;

public class GetGenrePlaylistsEndpoint(GenrePlaylistService genrePlaylistService) : EndpointWithoutRequest<GetGenrePlaylistsResponse>
{
    private readonly GenrePlaylistService _genrePlaylistService = genrePlaylistService;

    public override void Configure()
    {
        Get("");
        Group<GenrePlaylistGroup>();
        Policies(PolicyNames.HasAdminOrUserRole);
    }

    public override async Task<GetGenrePlaylistsResponse> ExecuteAsync(CancellationToken ct)
    {
        var playlists = await _genrePlaylistService.GetAllAsync(ct);

        return new GetGenrePlaylistsResponse
        {
            Result = playlists
        };
    }
}
