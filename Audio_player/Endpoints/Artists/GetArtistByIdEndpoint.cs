using Audio_player.Constants;
using Audio_player.Models.DTOs;
using Audio_player.Services;
using FastEndpoints;

namespace Audio_player.Endpoints.Artists;

public class GetArtistByIdEndpoint(ArtistService artistService) : EndpointWithoutRequest<ArtistDTO?>
{
    private readonly ArtistService _artistService = artistService;

    public override void Configure()
    {
        Get("{id:int}");
        Group<ArtistGroup>();
        Policies(PolicyNames.HasAdminOrUserRole);
    }

    public override async Task<ArtistDTO?> ExecuteAsync(CancellationToken ct)
    {
        var id = Route<long>("id");

        var artist = await _artistService.GetByIdAsync(id, HttpContext.User, ct);

        if (artist == null)
        {
            await SendNotFoundAsync(ct);
            return null;
        }

        return artist;
    }
}
