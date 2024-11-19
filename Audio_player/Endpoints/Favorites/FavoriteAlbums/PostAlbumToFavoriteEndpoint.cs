using Audio_player.Constants;
using Audio_player.DAL;
using Audio_player.Models.Responses;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Audio_player.Endpoints.Favorites.FavoriteAlbums;

public class PostAlbumToFavoriteEndpoint(AppDbContext appDbContext) : EndpointWithoutRequest<FavoriteResponse>
{
    private readonly AppDbContext _appDbContext = appDbContext;

    public override void Configure()
    {
        Post("{id:int}");
        Group<FavoriteAlbumsGroup>();
    }

    public override async Task<FavoriteResponse> ExecuteAsync(CancellationToken ct)
    {
        var albumId = Route<long>("id");

        if (!_appDbContext.Albums.Any(x => x.Id == albumId))
        {
            await SendNotFoundAsync(ct);
            return new FavoriteResponse
            {
                Added = false
            };
        }

        var email = HttpContext.User.Claims.
          FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value!;
        var userId = await _appDbContext.AppUsers.Where(x => x.Email == email)
                .Select(x => x.UserProfile!.Id)
        .SingleOrDefaultAsync(ct);

        _appDbContext.UserAlbums.Add(new DAL.Models.UserAlbum
        {
            AlbumId = albumId,
            UserId = userId
        });

        await _appDbContext.SaveChangesAsync(ct);

        return new FavoriteResponse
        {
            Added = true
        };
    }
}
