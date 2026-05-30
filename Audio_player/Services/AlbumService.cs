using Audio_player.DAL;
using Audio_player.DAL.Models;
using Audio_player.Extensions;
using Audio_player.Mappers;
using Audio_player.Models.DTOs;
using Audio_player.Models.Requests;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Audio_player.Services;

public class AlbumService(AppDbContext appDbContext, FileService fileService)
{
    private readonly AppDbContext _appDbContext = appDbContext;
    private readonly FileService _fileService = fileService;

    public async Task<List<AlbumDTO>> GetByNameAsync(string? name, ClaimsPrincipal user, CancellationToken ct)
    {
        var userId = await _appDbContext.GetCurrentUserProfileIdAsync(user, ct);

        return await _appDbContext.Albums
            .Where(x => EF.Functions.ILike(x.AlbumName, $"%{name}%"))
            .EntityToDto(userId)
            .ToListAsync(ct);
    }

    public async Task<List<AlbumDTO>> GetByArtistIdAsync(long artistId, ClaimsPrincipal user, CancellationToken ct)
    {
        var userId = await _appDbContext.GetCurrentUserProfileIdAsync(user, ct);

        return await _appDbContext.Albums
            .Where(x => x.ArtistsAlbums.Any(a => a.ArtistId == artistId))
            .EntityToDto(userId)
            .ToListAsync(ct);
    }

    public async Task<AlbumDTO?> GetByIdAsync(long id, ClaimsPrincipal user, CancellationToken ct)
    {
        var userId = await _appDbContext.GetCurrentUserProfileIdAsync(user, ct);

        return await _appDbContext.Albums
            .Where(x => x.Id == id)
            .EntityToDto(userId)
            .SingleOrDefaultAsync(ct);
    }

    public async Task CreateAsync(PostAlbumRequest req, CancellationToken ct)
    {
        var coverPath = await _fileService.CreateFile(req.Cover, true, ct);

        var album = _appDbContext.Albums.Add(new Album
        {
            AlbumName = req.AlbumName,
            CoverPath = coverPath
        });

        await _appDbContext.SaveChangesAsync(ct);

        _appDbContext.GenreAlbums.AddRange(
            req.GenreIds.Select(genreId => new GenreAlbum { GenreId = genreId, AlbumId = album.Entity.Id }));

        _appDbContext.ArtistAlbums.AddRange(
            req.ArtistIds.Select(artistId => new ArtistAlbum { ArtistId = artistId, AlbumId = album.Entity.Id }));

        await _appDbContext.SaveChangesAsync(ct);
    }

    public async Task<bool> UpdateAsync(long id, PutAlbumRequest req, CancellationToken ct)
    {
        var album = await _appDbContext.Albums
            .Include(x => x.GenreAlbums)
            .Include(x => x.ArtistsAlbums)
            .SingleOrDefaultAsync(x => x.Id == id, ct);

        if (album == null)
        {
            return false;
        }

        album.AlbumName = req.AlbumName;

        if (req.Cover != null)
        {
            album.CoverPath = await _fileService.CreateFile(req.Cover, true, ct);
        }

        var genreIds = req.GenreIds ?? [];
        _appDbContext.GenreAlbums.RemoveRange(
            album.GenreAlbums.Where(x => !genreIds.Contains(x.GenreId)));
        _appDbContext.GenreAlbums.AddRange(
            genreIds.Where(genreId => album.GenreAlbums.All(x => x.GenreId != genreId))
                .Select(genreId => new GenreAlbum { GenreId = genreId, AlbumId = album.Id }));

        var artistIds = req.ArtistIds ?? [];
        _appDbContext.ArtistAlbums.RemoveRange(
            album.ArtistsAlbums.Where(x => !artistIds.Contains(x.ArtistId)));
        _appDbContext.ArtistAlbums.AddRange(
            artistIds.Where(artistId => album.ArtistsAlbums.All(x => x.ArtistId != artistId))
                .Select(artistId => new ArtistAlbum { AlbumId = album.Id, ArtistId = artistId }));

        await _appDbContext.SaveChangesAsync(ct);
        return true;
    }

    public async Task<bool> DeleteAsync(long id, CancellationToken ct)
    {
        var album = await _appDbContext.Albums.SingleOrDefaultAsync(x => x.Id == id, ct);

        if (album == null)
        {
            return false;
        }

        _appDbContext.Albums.Remove(album);
        _fileService.DeleteFile(album.CoverPath);

        await _appDbContext.SaveChangesAsync(ct);
        return true;
    }
}
