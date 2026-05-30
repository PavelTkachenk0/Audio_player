using Audio_player.Constants;
using Audio_player.Models.DTOs;
using Audio_player.Services;
using FastEndpoints;

namespace Audio_player.Endpoints.GenrePlaylists;

public class GetGenrePlaylistsByIdEndpoint(GenrePlaylistService genrePlaylistService) : EndpointWithoutRequest<GenrePlaylistDTO?>
{
    private readonly GenrePlaylistService _genrePlaylistService = genrePlaylistService;

    public override void Configure()
    {
        Get("{id:int}");
        Group<GenrePlaylistGroup>();
        Policies(PolicyNames.HasAdminOrUserRole);
    }

    public override async Task<GenrePlaylistDTO?> ExecuteAsync(CancellationToken ct)
    {
        var id = Route<long>("id");

        var playlist = await _genrePlaylistService.GetByIdAsync(id, HttpContext.User, ct);

        if (playlist == null)
        {
            await SendNotFoundAsync(ct);
            return null;
        }

        return playlist;
    }
}
