using Audio_player.Constants;
using Audio_player.DAL;
using Audio_player.Models.DTOs;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Audio_player.Endpoints.GenrePlaylists;

public class GetGenrePlaylistsByIdEndpoint(AppDbContext appDbContext) : EndpointWithoutRequest<GenrePlaylistDTO?>
{
    private readonly AppDbContext _appDbContext = appDbContext;

    public override void Configure()
    {
        Get("{id:int}");
        Group<GenrePlaylistGroup>();
        Policies(PolicyNames.HasAdminOrUserRole);
    }

    public override async Task<GenrePlaylistDTO?> ExecuteAsync(CancellationToken ct)
    {
        var id = Route<long>("id");

        var email = HttpContext.User.Claims.
         FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value!;
        var userId = await _appDbContext.AppUsers.Where(x => x.Email == email)
                .Select(x => x.UserProfile!.Id)
                .SingleOrDefaultAsync(ct);

        var playlist = await _appDbContext.Playlists.Select(x => new GenrePlaylistDTO
        {
            Id = x.Id,
            CoverPath = x.CoverPath,
            Name = x.Name,
            Songs = x.Songs.Select(s => new TrackForPlaylistDTO
            {
                Id = s.Id,
                SongPath = s.SongPath,
                SongName = s.SongName,
                Duration = s.Duration,
                IsFavorite = s.UserSongs.Any(us => us.UserId == userId),
                CoverPath = s.Album!.CoverPath,
                ListeningCount = s.ListeningCount,
                Artists = s.Artists.Select(x => new ShortArtistDTO
                {
                    ArtistName = x.ArtistName,
                    Id = x.Id,
                }).ToList(),
            }).ToList()
        }).SingleOrDefaultAsync(x => x.Id == id, ct);

        if (playlist == null)
        {
            await SendNotFoundAsync(ct);
            return null;
        }

        return playlist;
    }
}
