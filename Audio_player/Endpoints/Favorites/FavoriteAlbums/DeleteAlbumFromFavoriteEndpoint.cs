using Audio_player.Constants;
using Audio_player.DAL;
using Audio_player.Models.Responses;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Audio_player.Endpoints.Favorites.FavoriteAlbums;

public class DeleteAlbumFromFavoriteEndpoint(AppDbContext appDbContext) : EndpointWithoutRequest<FavoriteResponse>
{
    private readonly AppDbContext _appDbContext = appDbContext;

    public override void Configure()
    {
        Delete("{id:int}");
        Group<FavoriteAlbumsGroup>();
    }

    public override async Task<FavoriteResponse> ExecuteAsync(CancellationToken ct)
    {
        var albumId = Route<long>("id");

        var email = HttpContext.User.Claims.
          FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value!;
        var userId = await _appDbContext.AppUsers.Where(x => x.Email == email)
                .Select(x => x.UserProfile!.Id)
        .SingleOrDefaultAsync(ct);

        var album = await _appDbContext.UserAlbums.SingleOrDefaultAsync(x => x.UserId == userId && x.AlbumId == albumId, ct);

        if (album == null)
        {
            await SendNotFoundAsync(ct);
            return new FavoriteResponse
            {
                Added = false
            };
        }

        _appDbContext.UserAlbums.Remove(album);

        await _appDbContext.SaveChangesAsync(ct);

        return new FavoriteResponse
        {
            Added = true
        };
    }
}
