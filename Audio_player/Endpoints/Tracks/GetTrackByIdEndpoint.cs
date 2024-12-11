using Audio_player.Constants;
using Audio_player.DAL;
using Audio_player.Models.DTOs;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Audio_player.Endpoints.Tracks;

public class GetTrackByIdEndpoint(AppDbContext appDbContext) : EndpointWithoutRequest<TrackDTO?>
{
    private readonly AppDbContext _appDbContext = appDbContext;

    public override void Configure()
    {
        Get("{id:int}");
        Group<TrackGroup>();
        Policies(PolicyNames.HasAdminOrUserRole);
    }

    public override async Task<TrackDTO?> ExecuteAsync(CancellationToken ct)
    {
        var id = Route<long>("id");

        var email = HttpContext.User.Claims.
          FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value!;
        var userId = await _appDbContext.AppUsers.Where(x => x.Email == email)
                .Select(x => x.UserProfile!.Id)
                .SingleOrDefaultAsync(ct);

        var track = await _appDbContext.Songs
            .Select(x => new TrackDTO
            {
                Id = x.Id,
                Duration = x.Duration,
                ListeningCount = x.ListeningCount,
                SongName = x.SongName,
                SongPath = x.SongPath,
                IsFavorite = x.UserSongs.Any(x => x.UserId == userId),
                Album = new ShortAlbumDTO
                {
                    AlbumName = x.Album!.AlbumName,
                    CoverPath = x.Album!.CoverPath,
                    Id = x.Album!.Id
                },
                Genres = x.Genres.Select(g => new ShortGenreDTO
                {
                    Id = g.Id,
                    Name = g.Name
                }).ToList(),
                Artists = x.Artists.Select(a => new ShortArtistDTO
                {
                    ArtistName = a.ArtistName,
                    Id = a.Id
                }).ToList()
            }).SingleOrDefaultAsync(x => x.Id == id, ct);

        if (track == null)
        {
            await SendNotFoundAsync(ct);
            return null;
        }

        return track;
    }
}
