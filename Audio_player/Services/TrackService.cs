using Audio_player.DAL;
using Audio_player.DAL.Models;
using Audio_player.Extensions;
using Audio_player.Mappers;
using Audio_player.Models.DTOs;
using Audio_player.Models.Requests;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Audio_player.Services;

public class TrackService(AppDbContext appDbContext, FileService fileService)
{
    private readonly AppDbContext _appDbContext = appDbContext;
    private readonly FileService _fileService = fileService;

    public async Task<List<TrackDTO>> GetByNameAsync(string? name, ClaimsPrincipal user, CancellationToken ct)
    {
        var userId = await _appDbContext.GetCurrentUserProfileIdAsync(user, ct);

        return await _appDbContext.Songs
            .Where(x => EF.Functions.ILike(x.SongName, $"%{name}%"))
            .EntityToDto(userId)
            .ToListAsync(ct);
    }

    public async Task<TrackDTO?> GetByIdAsync(long id, ClaimsPrincipal user, CancellationToken ct)
    {
        var userId = await _appDbContext.GetCurrentUserProfileIdAsync(user, ct);

        return await _appDbContext.Songs
            .Where(x => x.Id == id)
            .EntityToDto(userId)
            .SingleOrDefaultAsync(ct);
    }

    public async Task<List<TrackByAlbumIdDTO>> GetByAlbumIdAsync(long albumId, ClaimsPrincipal user, CancellationToken ct)
    {
        var userId = await _appDbContext.GetCurrentUserProfileIdAsync(user, ct);

        return await _appDbContext.Songs
            .Where(x => x.AlbumId == albumId)
            .EntityToByAlbumIdDto(userId)
            .ToListAsync(ct);
    }

    public async Task CreateAsync(PostTrackRequest req, CancellationToken ct)
    {
        var songPath = await _fileService.CreateFile(req.SongFile, false, ct);

        var track = _appDbContext.Songs.Add(new Song
        {
            SongName = req.SongName,
            SongPath = songPath,
            AlbumId = req.AlbumId,
            Duration = req.Duration
        });

        await _appDbContext.SaveChangesAsync(ct);

        _appDbContext.GenreSongs.AddRange(
            req.GenreIds.Select(x => new GenreSong
            {
                GenreId = x,
                SongId = track.Entity.Id
            })
        );

        _appDbContext.ArtistSongs.AddRange(
            req.ArtistIds.Select(x => new ArtistSong
            {
                ArtistId = x,
                SongId = track.Entity.Id
            })
        );

        await _appDbContext.SaveChangesAsync(ct);
    }

    public async Task<bool> UpdateAsync(long id, PutTrackRequest req, CancellationToken ct)
    {
        var track = await _appDbContext.Songs
            .Include(x => x.GenreSongs)
            .Include(x => x.ArtistSongs)
            .SingleOrDefaultAsync(x => x.Id == id, ct);

        if (track == null)
        {
            return false;
        }

        track.SongName = req.SongName;
        track.Duration = req.Duration;
        track.AlbumId = req.AlbumId;

        if (req.SongFile != null)
        {
            var songPath = await _fileService.CreateFile(req.SongFile, false, ct);
            track.SongPath = songPath;
        }

        var genresInRequest = req.GenreIds ?? [];

        var genresToDelete = track.GenreSongs
            .Where(x => !genresInRequest.Contains(x.GenreId) && x.SongId == track.Id);

        var genresToAdd = req.GenreIds?
            .Where(id => !track.GenreSongs.Any(x => x.GenreId == id))
            .Select(x => new GenreSong
            {
                GenreId = x,
                SongId = track.Id
            }) ?? [];

        _appDbContext.GenreSongs.RemoveRange(genresToDelete);

        _appDbContext.GenreSongs.AddRange(genresToAdd);

        var artistsInRequest = req.ArtistIds ?? [];

        var artistsToDelete = track.ArtistSongs
            .Where(x => !artistsInRequest.Contains(x.ArtistId) && x.SongId == track.Id);

        var artistsToAdd = req.ArtistIds?
            .Where(id => !track.ArtistSongs.Any(x => x.ArtistId == id))
            .Select(x => new ArtistSong
            {
                SongId = track.Id,
                ArtistId = x
            }) ?? [];

        _appDbContext.ArtistSongs.RemoveRange(artistsToDelete);

        _appDbContext.ArtistSongs.AddRange(artistsToAdd);

        await _appDbContext.SaveChangesAsync(ct);

        return true;
    }

    public async Task<bool> DeleteAsync(long id, CancellationToken ct)
    {
        var track = await _appDbContext.Songs.SingleOrDefaultAsync(x => x.Id == id, ct);

        if (track == null)
        {
            return false;
        }

        _appDbContext.Songs.Remove(track);

        _fileService.DeleteFile(track.SongPath);

        await _appDbContext.SaveChangesAsync(ct);
        return true;
    }
}
