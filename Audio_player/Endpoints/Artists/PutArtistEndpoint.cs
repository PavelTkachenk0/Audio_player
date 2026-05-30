using Audio_player.Constants;
using Audio_player.Models.Requests;
using Audio_player.Services;
using FastEndpoints;

namespace Audio_player.Endpoints.Artists;

public class PutArtistEndpoint(ArtistService artistService) : Endpoint<PutArtistRequest>
{
    private readonly ArtistService _artistService = artistService;

    public override void Configure()
    {
        Put("{id:int}");
        Group<ArtistGroup>();
        AllowFileUploads();
        Policies(PolicyNames.HasAdminRole);
    }

    public override async Task HandleAsync(PutArtistRequest req, CancellationToken ct)
    {
        var id = Route<long>("id");

        if (!await _artistService.UpdateAsync(id, req, ct))
        {
            await SendNotFoundAsync(ct);
            return;
        }

        await SendOkAsync(ct);
    }
}
