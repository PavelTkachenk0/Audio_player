using Audio_player.DAL;
using Audio_player.DAL.Models;
using Audio_player.Extensions;
using Audio_player.Mappers;
using Audio_player.Models.DTOs;
using Audio_player.Models.Requests;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Audio_player.Services;

public class GenrePlaylistService(AppDbContext appDbContext, FileService fileService)
{
    private readonly AppDbContext _appDbContext = appDbContext;
    private readonly FileService _fileService = fileService;

    public async Task<List<ShortGenrePlaylistDTO>> GetAllAsync(CancellationToken ct)
    {
        return await _appDbContext.Playlists
            .Where(x => x.IsAdmin)
            .EntityToShortDto()
            .ToListAsync(ct);
    }

    public async Task<GenrePlaylistDTO?> GetByIdAsync(long id, ClaimsPrincipal user, CancellationToken ct)
    {
        var userId = await _appDbContext.GetCurrentUserProfileIdAsync(user, ct);

        return await _appDbContext.Playlists
            .Where(x => x.Id == id)
            .EntityToDto(userId)
            .SingleOrDefaultAsync(ct);
    }

    public async Task CreateAsync(PostPlaylistRequest req, CancellationToken ct)
    {
        var coverPath = await _fileService.CreateFile(req.Cover, true, ct);

        var playlist = _appDbContext.Playlists.Add(new Playlist
        {
            Name = req.Name,
            CoverPath = coverPath,
            IsAdmin = true
        });

        await _appDbContext.SaveChangesAsync(ct);

        _appDbContext.PlaylistSongs.AddRange(
            req.TrackIds.Select(x => new PlaylistSong
            {
                SongId = x,
                PlaylistId = playlist.Entity.Id
            }));

        await _appDbContext.SaveChangesAsync(ct);
    }

    public async Task<bool> UpdateAsync(long id, PutPlaylistRequest req, CancellationToken ct)
    {
        var playlist = await _appDbContext.Playlists
            .Include(x => x.PlaylistSongs)
            .SingleOrDefaultAsync(x => x.Id == id, ct);

        if (playlist == null)
        {
            return false;
        }

        playlist.Name = req.Name;

        if (req.Cover != null)
        {
            var coverPath = await _fileService.CreateFile(req.Cover, true, ct);
            playlist.CoverPath = coverPath;
        }

        var songsInRequest = req.TrackIds ?? [];

        var songsToDelete = playlist.PlaylistSongs
            .Where(x => !songsInRequest.Contains(x.SongId) && x.PlaylistId == playlist.Id);

        var songsToAdd = req.TrackIds?
            .Where(id => !playlist.PlaylistSongs.Any(x => x.SongId == id))
            .Select(x => new PlaylistSong
            {
                SongId = x,
                PlaylistId = playlist.Id
            }) ?? [];

        _appDbContext.PlaylistSongs.RemoveRange(songsToDelete);

        _appDbContext.PlaylistSongs.AddRange(songsToAdd);

        await _appDbContext.SaveChangesAsync(ct);

        return true;
    }

    public async Task<bool> DeleteAsync(long id, CancellationToken ct)
    {
        var playlist = await _appDbContext.Playlists.SingleOrDefaultAsync(x => x.Id == id, ct);

        if (playlist == null)
        {
            return false;
        }

        _appDbContext.Playlists.Remove(playlist);

        _fileService.DeleteFile(playlist.CoverPath);

        await _appDbContext.SaveChangesAsync(ct);
        return true;
    }
}
