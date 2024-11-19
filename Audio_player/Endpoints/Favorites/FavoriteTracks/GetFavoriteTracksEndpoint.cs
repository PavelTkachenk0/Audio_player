using Audio_player.Constants;
using Audio_player.DAL;
using Audio_player.Endpoints.Favorites.Tracks;
using Audio_player.Models.DTOs;
using Audio_player.Models.Responses;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Audio_player.Endpoints.Favorites.FavoriteTracks;

public class GetFavoriteTracksEndpoint(AppDbContext appDbContext) : EndpointWithoutRequest<GetFavoriteTracksResponse>
{
    private readonly AppDbContext _appDbContext = appDbContext;

    public override void Configure()
    {
        Get("");
        Group<FavoriteTracksGroup>();
    }

    public override async Task<GetFavoriteTracksResponse> ExecuteAsync(CancellationToken ct)
    {
        var email = HttpContext.User.Claims.
         FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value!;
        var userId = await _appDbContext.AppUsers.Where(x => x.Email == email)
        .Select(x => x.UserProfile!.Id)
                .SingleOrDefaultAsync(ct);

        var tracks = await _appDbContext.Songs
            .Where(x => x.UserSongs.Select(us => us.SongId).Contains(x.Id))
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

        return new GetFavoriteTracksResponse
        {
            Result = tracks
        };
    }
}
