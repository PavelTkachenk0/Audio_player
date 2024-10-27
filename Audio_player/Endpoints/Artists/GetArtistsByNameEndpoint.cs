using Audio_player.Constants;
using Audio_player.DAL;
using Audio_player.Models.DTOs;
using Audio_player.Models.Requests;
using Audio_player.Models.Responses;
using AutoMapper;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;

namespace Audio_player.Endpoints.Artists;

public class GetArtistsByNameEndpoint(AppDbContext appDbContext) : Endpoint<GetArtistByNameRequest, GetArtistsResponse>
{
    private readonly AppDbContext _appDbContext = appDbContext;

    public override void Configure()
    {
        Get("");
        Group<ArtistGroup>();
        Policies(PolicyNames.HasAdminOrUserRole);
    }

    public override async Task<GetArtistsResponse> ExecuteAsync(GetArtistByNameRequest req, CancellationToken ct)
    {
        var name = req.Name?.ToLower();

        var artists = await _appDbContext.Artists
            .Where(x => EF.Functions.Like(x.ArtistName, $"%{name}%"))
            .Select(x => new ArtistDTO
            {
                ArtistName = x.ArtistName,
                CoverPath = x.CoverPath,
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
