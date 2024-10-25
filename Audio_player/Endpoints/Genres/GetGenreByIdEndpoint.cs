using Audio_player.Constants;
using Audio_player.DAL;
using Audio_player.Models.DTOs;
using Audio_player.Models.Responses;
using AutoMapper;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;

namespace Audio_player.Endpoints.Genres;

public class GetGenreByIdEndpoint(AppDbContext appDbContext, AutoMapper.IMapper mapper) : EndpointWithoutRequest<GenreDTO?>
{
    private readonly AppDbContext _appDbContext = appDbContext;
    private readonly AutoMapper.IMapper _mapper = mapper;

    public override void Configure()
    {
        Get("{id:int}");
        Group<GenreGroup>();
        Policies(PolicyNames.HasAdminOrUserRole);
    }

    public override async Task<GenreDTO?> ExecuteAsync(CancellationToken ct)
    {
        var genreId = Route<int>("id");

        var genre = await _appDbContext.Genres.SingleOrDefaultAsync(x => x.Id ==  genreId, ct);

        if (genre == null)
        {
            await SendNotFoundAsync(ct);
            return null;
        }

        return _mapper.Map<GenreDTO>(genre);
    }
}
