using Audio_player.Constants;
using Audio_player.DAL;
using Audio_player.Services;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;

namespace Audio_player.Endpoints.Albums;

public class DeleteAlbumEndpoint(AppDbContext appDbContext, FileService fileService) : EndpointWithoutRequest
{
    private readonly AppDbContext _appDbContext = appDbContext;
    private readonly FileService _fileService = fileService;

    public override void Configure()
    {
        Delete("{id:int}");
        Group<AlbumGroup>();
        Policies(PolicyNames.HasAdminRole);
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var id = Route<long>("id");

        var album = await _appDbContext.Albums.SingleOrDefaultAsync(x => x.Id == id, ct);

        if (album == null)
        {
            await SendNotFoundAsync(ct);
            return;
        }

        _appDbContext.Albums.Remove(album);

        _fileService.DeleteFile(album.CoverPath);

        await _appDbContext.SaveChangesAsync(ct);
        await SendOkAsync(ct);
    }
}
