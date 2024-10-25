using Audio_player.Constants;
using Audio_player.DAL;
using Audio_player.Models.DTOs;
using Audio_player.Models.Requests;
using Audio_player.Models.Responses;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;

namespace Audio_player.Endpoints.Genres;

public class GetGenresByNameEndpoint(AppDbContext appDbContext, AutoMapper.IMapper mapper) : Endpoint<GetGenresByNameRequest, GetGenresResponse>
{
    private readonly AppDbContext _appDbContext = appDbContext;
    private readonly AutoMapper.IMapper _mapper = mapper;

    public override void Configure()
    {
        Get("");
        Group<GenreGroup>();
        Policies(PolicyNames.HasAdminOrUserRole);
    }

    public override async Task<GetGenresResponse> ExecuteAsync(GetGenresByNameRequest req, CancellationToken ct)
    {
        var name = req.Name?.ToLower();

        var genres = await _appDbContext.Genres
            .Where(x => EF.Functions.Like(x.Name, $"%{name}%"))
            .ToListAsync(ct);

        return new GetGenresResponse
        {
            Result = _mapper.Map<List<GenreDTO>>(genres)
        };
    }
}
