using Audio_player.Constants;
using Audio_player.DAL;
using Audio_player.Models.DTOs;
using Audio_player.Models.Requests;
using Audio_player.Models.Responses;
using AutoMapper;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Audio_player.Endpoints.Artists;

public class GetArtistsByNameEndpoint(AppDbContext appDbContext) : Endpoint<GetByNameRequest, GetArtistsResponse>
{
    private readonly AppDbContext _appDbContext = appDbContext;

    public override void Configure()
    {
        Get("");
        Group<ArtistGroup>();
        Policies(PolicyNames.HasAdminOrUserRole);
    }

    public override async Task<GetArtistsResponse> ExecuteAsync(GetByNameRequest req, CancellationToken ct)
    {
        var email = HttpContext.User.Claims.
           FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value!;
        var userId = await _appDbContext.AppUsers.Where(x => x.Email == email)
                .Select(x => x.UserProfile!.Id)
                .SingleOrDefaultAsync(ct);

        var artists = await _appDbContext.Artists
            .Where(x => EF.Functions.ILike(x.ArtistName, $"%{req.Name}%"))
            .Select(x => new ArtistDTO
            {
                ArtistName = x.ArtistName,
                CoverPath = x.CoverPath,
                AvatarPath = x.AvatarPath,
                IsFavorite = x.UserArtists.Any(x => x.UserId == userId),
                Genres = x.Genres.Select(x => new ShortGenreDTO
                {
                    Id = x.Id,
                    Name = x.Name
                }).ToList(),
                Id = x.Id
            })
            .ToListAsync(ct);

        return new GetArtistsResponse
        {
            Result = artists
        };
    }
}
