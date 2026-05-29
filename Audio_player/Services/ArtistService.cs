using Audio_player.DAL;
using Audio_player.DAL.Models;
using Audio_player.Extensions;
using Audio_player.Mappers;
using Audio_player.Models.DTOs;
using Audio_player.Models.Requests;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Audio_player.Services;

public class ArtistService(AppDbContext appDbContext, FileService fileService)
{
    private readonly AppDbContext _appDbContext = appDbContext;
    private readonly FileService _fileService = fileService;

    public async Task<List<ArtistDTO>> GetByNameAsync(string? name, ClaimsPrincipal user, CancellationToken ct)
    {
        var userId = await _appDbContext.GetCurrentUserProfileIdAsync(user, ct);

        return await _appDbContext.Artists
            .Where(x => EF.Functions.ILike(x.ArtistName, $"%{name}%"))
            .EntityToDto(userId)
            .ToListAsync(ct);
    }

    public async Task<ArtistDTO?> GetByIdAsync(long id, ClaimsPrincipal user, CancellationToken ct)
    {
        var userId = await _appDbContext.GetCurrentUserProfileIdAsync(user, ct);

        return await _appDbContext.Artists
            .Where(x => x.Id == id)
            .EntityToDto(userId)
            .SingleOrDefaultAsync(ct);
    }

    public async Task CreateAsync(PostArtistRequest req, CancellationToken ct)
    {
        var coverPath = await _fileService.CreateFile(req.Cover, true, ct);
        var avatarPath = await _fileService.CreateFile(req.Avatar, true, ct);

        var artist = _appDbContext.Artists.Add(new Artist
        {
            ArtistName = req.ArtistName,
            CoverPath = coverPath,
            AvatarPath = avatarPath
        });

        await _appDbContext.SaveChangesAsync(ct);

        _appDbContext.GenreArtists.AddRange(
            req.GenreIds.Select(x => new GenreArtist
            {
                GenreId = x,
                ArtistId = artist.Entity.Id
            }));

        await _appDbContext.SaveChangesAsync(ct);
    }

    public async Task<bool> UpdateAsync(long id, PutArtistRequest req, CancellationToken ct)
    {
        var artist = await _appDbContext.Artists
            .Include(x => x.GenreArtists)
            .SingleOrDefaultAsync(x => x.Id == id, ct);

        if (artist == null)
        {
            return false;
        }

        artist.ArtistName = req.ArtistName;

        if (req.Cover != null)
        {
            var coverPath = await _fileService.CreateFile(req.Cover, true, ct);
            artist.CoverPath = coverPath;
        }

        if (req.Avatar != null)
        {
            var avatarPath = await _fileService.CreateFile(req.Avatar, true, ct);
            artist.AvatarPath = avatarPath;
        }

        var genresInRequest = req.GenreIds ?? [];

        var genresToDelete = artist.GenreArtists
            .Where(x => !genresInRequest.Contains(x.GenreId) && x.ArtistId == artist.Id);

        var genresToAdd = req.GenreIds?
            .Where(id => !artist.GenreArtists.Any(x => x.GenreId == id))
            .Select(x => new GenreArtist
            {
                GenreId = x,
                ArtistId = artist.Id
            }) ?? [];

        _appDbContext.GenreArtists.RemoveRange(genresToDelete);

        _appDbContext.GenreArtists.AddRange(genresToAdd);

        await _appDbContext.SaveChangesAsync(ct);

        return true;
    }

    public async Task<bool> DeleteAsync(long id, CancellationToken ct)
    {
        var artist = await _appDbContext.Artists.SingleOrDefaultAsync(x => x.Id == id, ct);

        if (artist == null)
        {
            return false;
        }

        _appDbContext.Artists.Remove(artist);

        _fileService.DeleteFile(artist.CoverPath);
        _fileService.DeleteFile(artist.AvatarPath);

        await _appDbContext.SaveChangesAsync(ct);
        return true;
    }
}
