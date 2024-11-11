using Audio_player.Constants;
using Audio_player.DAL;
using Audio_player.DAL.Models;
using Audio_player.Models.DTOs;
using Audio_player.Models.Requests;
using Audio_player.Models.Responses;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;

namespace Audio_player.Endpoints.Search;

public class SearchEndpoint(AppDbContext appDbContext) : Endpoint<SearchTermRequest, SearchResponse>
{
    private readonly AppDbContext _appDbContext = appDbContext;

    public override void Configure()
    {
        Get("");
        Group<SearchGroup>();
        Policies(PolicyNames.HasAdminOrUserRole);
    }

    public override async Task<SearchResponse> ExecuteAsync(SearchTermRequest req, CancellationToken ct)
    {
        var searchResult = await _appDbContext.Albums
            .Where(al => EF.Functions.ILike(al.AlbumName, $"%{req.SearchTerm}%"))
            .Select(al => new SearchDTO
            {
                CoverPath = al.CoverPath,
                Name = al.AlbumName,
                Source = nameof(Album),
                Id = al.Id
            }).Union(_appDbContext.Songs
                        .Where(s => EF.Functions.ILike(s.SongName, $"%{req.SearchTerm}%"))
                        .Select(s => new SearchDTO
                        {
                            Id = s.Id,
                            Name = s.SongName,
                            Source = nameof(Song),
                            CoverPath = s.Album!.CoverPath
                        })).Union(_appDbContext.Artists
                                .Where(x => EF.Functions.ILike(x.ArtistName, $"%{req.SearchTerm}%"))
                                .Select(ar => new SearchDTO
                                {
                                    Id = ar.Id,
                                    Source = nameof(Artist),
                                    CoverPath = ar.CoverPath,
                                    Name = ar.ArtistName
                                })).ToListAsync(ct);

        return new SearchResponse
        {
            Result = searchResult
        };
    }
}
