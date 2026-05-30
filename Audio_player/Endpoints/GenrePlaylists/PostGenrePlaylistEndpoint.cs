using Audio_player.Constants;
using Audio_player.Models.Requests;
using Audio_player.Services;
using FastEndpoints;

namespace Audio_player.Endpoints.GenrePlaylists;

public class PostGenrePlaylistEndpoint(GenrePlaylistService genrePlaylistService) : Endpoint<PostPlaylistRequest>
{
    private readonly GenrePlaylistService _genrePlaylistService = genrePlaylistService;

    public override void Configure()
    {
        Post("");
        Group<GenrePlaylistGroup>();
        AllowFileUploads();
        Policies(PolicyNames.HasAdminRole);
    }

    public override async Task HandleAsync(PostPlaylistRequest req, CancellationToken ct)
    {
        await _genrePlaylistService.CreateAsync(req, ct);
        await SendOkAsync(ct);
    }
}
