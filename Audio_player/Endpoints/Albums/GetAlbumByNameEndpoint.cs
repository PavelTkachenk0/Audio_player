using Audio_player.Constants;
using Audio_player.DAL;
using Audio_player.Models.DTOs;
using Audio_player.Models.Requests;
using Audio_player.Models.Responses;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Audio_player.Endpoints.Albums;

public class GetAlbumByNameEndpoint(AppDbContext appDbContext) : Endpoint<GetByNameRequest, GetAlbumsResponse>
{
    private readonly AppDbContext _appDbContext = appDbContext;

    public override void Configure()
    {
        Get("");
        Group<AlbumGroup>();
        Policies(PolicyNames.HasAdminOrUserRole);
    }

    public override async Task<GetAlbumsResponse> ExecuteAsync(GetByNameRequest req, CancellationToken ct)
    {
        var email = HttpContext.User.Claims.
          FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value!;
        var userId = await _appDbContext.AppUsers.Where(x => x.Email == email)
                .Select(x => x.UserProfile!.Id)
                .SingleOrDefaultAsync(ct);

        var albums = await _appDbContext.Albums
            .Where(x => EF.Functions.ILike(x.AlbumName, $"%{req.Name}%"))
            .Select(x => new AlbumDTO
            {
                AlbumName = x.AlbumName,
                CoverPath = x.CoverPath,
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
                IsFavorite = x.UserAlbums.Any(x => x.UserId == userId),
            }).ToListAsync(ct);

        return new GetAlbumsResponse
        {
            Result = albums
        };
    }
}
