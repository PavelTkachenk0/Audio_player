using Audio_player.Constants;
using Audio_player.DAL;
using Audio_player.Models.DTOs;
using Audio_player.Models.Responses;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;

namespace Audio_player.Endpoints.GenrePlaylists;

public class GetGenrePlaylists(AppDbContext appDbContext) : EndpointWithoutRequest<GetGenrePlaylistsResponse>
{
    private readonly AppDbContext _appDbContext = appDbContext;

    public override void Configure()
    {
        Get("");
        Group<GenrePlaylistGroup>();
        Policies(PolicyNames.HasAdminOrUserRole);
    }

    public override async Task<GetGenrePlaylistsResponse> ExecuteAsync(CancellationToken ct)
    {
        var playlists = await _appDbContext.Playlists.Where(x => x.IsAdmin).Select(x => new GenrePlaylistDTO
        {
            CoverPath = x.CoverPath,
            Id = x.Id,
            Name = x.Name
        }).ToListAsync(ct);

        return new GetGenrePlaylistsResponse
        {
            Result = playlists
        };
    }
}
