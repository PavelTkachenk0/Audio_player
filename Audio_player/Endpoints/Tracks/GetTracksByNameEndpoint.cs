using Audio_player.Constants;
using Audio_player.DAL;
using Audio_player.Models.DTOs;
using Audio_player.Models.Requests;
using Audio_player.Models.Responses;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Audio_player.Endpoints.Tracks;

public class GetTracksByNameEndpoint(AppDbContext appDbContext) : Endpoint<GetByNameRequest, GetTracksResponse>
{
    private readonly AppDbContext _appDbContext = appDbContext;

    public override void Configure()
    {
        Get("");
        Group<TrackGroup>();
        Policies(PolicyNames.HasAdminOrUserRole);
    }

    public override async Task<GetTracksResponse> ExecuteAsync(GetByNameRequest req, CancellationToken ct)
    {
        var email = HttpContext.User.Claims.
         FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value!;
        var userId = await _appDbContext.AppUsers.Where(x => x.Email == email)
                .Select(x => x.UserProfile!.Id)
                .SingleOrDefaultAsync(ct);

        var tracks = await _appDbContext.Songs
            .Where(x => EF.Functions.ILike(x.SongName, $"%{req.Name}%"))
            .Select(x => new TrackDTO
            {
                SongName = x.SongName,
                SongPath = x.SongPath,
                Genres = x.Genres.Select(x => new ShortGenreDTO
                {
                    Id = x.Id,
                    Name = x.Name
                }).ToList(),
                Artists = x.Artists.Select(x => new ShortArtistDTO
                {
                    ArtistName = x.ArtistName,
                    Id = x.Id,
                }).ToList(),
                Id = x.Id,
                Duration = x.Duration,
                ListeningCount = x.ListeningCount,
                Album = new ShortAlbumDTO
                {
                    AlbumName = x.Album!.AlbumName,
                    CoverPath = x.Album!.CoverPath,
                    Id = x.Id
                },
                IsFavorite = x.UserSongs.Any(x => x.UserId == userId),
            }).ToListAsync(ct);

        return new GetTracksResponse
        {
            Result = tracks
        };
    }
}
