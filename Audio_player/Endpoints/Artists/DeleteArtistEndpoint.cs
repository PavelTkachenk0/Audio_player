using Audio_player.Constants;
using Audio_player.DAL;
using Audio_player.Services;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;

namespace Audio_player.Endpoints.Artists;

public class DeleteArtistEndpoint(AppDbContext appDbContext, FileService fileService) : EndpointWithoutRequest
{
    private readonly AppDbContext _appDbContext = appDbContext;
    private readonly FileService _fileService = fileService;

    public override void Configure()
    {
        Delete("{id:int}");
        Group<ArtistGroup>();
        Policies(PolicyNames.HasAdminRole);
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var id = Route<long>("id");

        var artist = await _appDbContext.Artists.SingleOrDefaultAsync(x => x.Id == id, ct);

        if (artist == null)
        {
            await SendNotFoundAsync(ct);
            return;
        }

        _appDbContext.Artists.Remove(artist);

        _fileService.DeleteFile(artist.CoverPath);
        _fileService.DeleteFile(artist.AvatarPath);
        
        await _appDbContext.SaveChangesAsync(ct);
        await SendOkAsync(ct);
    }
}
