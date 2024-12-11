using Audio_player.Constants;
using Audio_player.DAL;
using Audio_player.Endpoints.Favorites.Tracks;
using Audio_player.Models.Responses;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Audio_player.Endpoints.Favorites.FavoriteTracks;

public class DeleteTrackFromFavoriteEndpoint(AppDbContext appDbContext) : EndpointWithoutRequest<FavoriteResponse>
{
    private readonly AppDbContext _appDbContext = appDbContext;

    public override void Configure()
    {
        Delete("{id:int}");
        Group<FavoriteTracksGroup>();
    }

    public override async Task<FavoriteResponse> ExecuteAsync(CancellationToken ct)
    {
        var trackId = Route<long>("id");

        var email = HttpContext.User.Claims.
          FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value!;
        var userId = await _appDbContext.AppUsers.Where(x => x.Email == email)
                .Select(x => x.UserProfile!.Id)
        .SingleOrDefaultAsync(ct);

        var track = await _appDbContext.UserSongs.SingleOrDefaultAsync(x => x.UserId == userId && x.SongId == trackId, ct);

        if (track == null)
        {
            await SendNotFoundAsync(ct);
            return new FavoriteResponse
            {
                Added = false
            };
        }

        _appDbContext.UserSongs.Remove(track);

        await _appDbContext.SaveChangesAsync(ct);

        return new FavoriteResponse
        {
            Added = true
        };
    }
}
