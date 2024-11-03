using Audio_player.Constants;
using Audio_player.DAL;
using Audio_player.Models.DTOs;
using Audio_player.Models.Responses;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Audio_player.Endpoints.Tracks;

public class GetTrackByAlbumIdEndpoint(AppDbContext appDbContext) : EndpointWithoutRequest<GetTracksByAlbumIdResponse>
{
    private readonly AppDbContext _appDbContext = appDbContext;

    public override void Configure()
    {
        Get("by-albumId/{albumId:int}");
        Group<TrackGroup>();
        Policies(PolicyNames.HasAdminOrUserRole);
    }

    public override async Task<GetTracksByAlbumIdResponse> ExecuteAsync(CancellationToken ct)
    {
        var albumId = Route<long>("albumId");

        var email = HttpContext.User.Claims.
        FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value!;
        var userId = await _appDbContext.AppUsers.Where(x => x.Email == email)
                .Select(x => x.UserProfile!.Id)
                .SingleOrDefaultAsync(ct);

        var tracks = await _appDbContext.Songs
            .Where(x => x.AlbumId == albumId)
            .Select(x => new TrackByAlbumIdDTO
            {
                Id = x.Id,
                Duration = x.Duration,
                IsFavorite = x.UserSongs.Any(x => x.UserId == userId),
                ListeningCount = x.ListeningCount,
                SongName = x.SongName,
                SongPath = x.SongPath,
                Artists = x.Artists.Select(a => new ShortArtistDTO
                {
                    ArtistName = a.ArtistName,
                    Id = a.Id,
                }).ToList()
            }).ToListAsync(ct);

        return new GetTracksByAlbumIdResponse
        {
            Result = tracks
        };
    }
}
