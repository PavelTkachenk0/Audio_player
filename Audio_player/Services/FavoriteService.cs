using Audio_player.DAL;
using Audio_player.DAL.Models;
using Audio_player.Extensions;
using Audio_player.Mappers;
using Audio_player.Models.DTOs;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Audio_player.Services;

public class FavoriteService(AppDbContext appDbContext)
{
    private readonly AppDbContext _appDbContext = appDbContext;

    // Tracks

    public async Task<List<TrackDTO>> GetFavoriteTracksAsync(ClaimsPrincipal user, CancellationToken ct)
    {
        var userId = await _appDbContext.GetCurrentUserProfileIdAsync(user, ct);

        return await _appDbContext.Songs
            .Where(x => x.UserSongs.Any(us => us.UserId == userId))
            .EntityToDto(userId)
            .ToListAsync(ct);
    }

    public async Task<bool> AddTrackAsync(long trackId, ClaimsPrincipal user, CancellationToken ct)
    {
        if (!await _appDbContext.Songs.AnyAsync(x => x.Id == trackId, ct))
        {
            return false;
        }

        var userId = await _appDbContext.GetCurrentUserProfileIdAsync(user, ct);

        _appDbContext.UserSongs.Add(new UserSongs
        {
            SongId = trackId,
            UserId = userId
        });

        await _appDbContext.SaveChangesAsync(ct);
        return true;
    }

    public async Task<bool> RemoveTrackAsync(long trackId, ClaimsPrincipal user, CancellationToken ct)
    {
        var userId = await _appDbContext.GetCurrentUserProfileIdAsync(user, ct);

        var track = await _appDbContext.UserSongs
            .SingleOrDefaultAsync(x => x.UserId == userId && x.SongId == trackId, ct);

        if (track == null)
        {
            return false;
        }

        _appDbContext.UserSongs.Remove(track);

        await _appDbContext.SaveChangesAsync(ct);
        return true;
    }

    // Albums

    public async Task<List<AlbumDTO>> GetFavoriteAlbumsAsync(ClaimsPrincipal user, CancellationToken ct)
    {
        var userId = await _appDbContext.GetCurrentUserProfileIdAsync(user, ct);

        return await _appDbContext.Albums
            .Where(x => x.UserAlbums.Any(ua => ua.UserId == userId))
            .EntityToDto(userId)
            .ToListAsync(ct);
    }

    public async Task<bool> AddAlbumAsync(long albumId, ClaimsPrincipal user, CancellationToken ct)
    {
        if (!await _appDbContext.Albums.AnyAsync(x => x.Id == albumId, ct))
        {
            return false;
        }

        var userId = await _appDbContext.GetCurrentUserProfileIdAsync(user, ct);

        _appDbContext.UserAlbums.Add(new UserAlbum
        {
            AlbumId = albumId,
            UserId = userId
        });

        await _appDbContext.SaveChangesAsync(ct);
        return true;
    }

    public async Task<bool> RemoveAlbumAsync(long albumId, ClaimsPrincipal user, CancellationToken ct)
    {
        var userId = await _appDbContext.GetCurrentUserProfileIdAsync(user, ct);

        var album = await _appDbContext.UserAlbums
            .SingleOrDefaultAsync(x => x.UserId == userId && x.AlbumId == albumId, ct);

        if (album == null)
        {
            return false;
        }

        _appDbContext.UserAlbums.Remove(album);

        await _appDbContext.SaveChangesAsync(ct);
        return true;
    }

    // Artists

    public async Task<List<ArtistDTO>> GetFavoriteArtistsAsync(ClaimsPrincipal user, CancellationToken ct)
    {
        var userId = await _appDbContext.GetCurrentUserProfileIdAsync(user, ct);

        return await _appDbContext.Artists
            .Where(x => x.UserArtists.Any(ua => ua.UserId == userId))
            .EntityToDto(userId)
            .ToListAsync(ct);
    }

    public async Task<bool> AddArtistAsync(long artistId, ClaimsPrincipal user, CancellationToken ct)
    {
        if (!await _appDbContext.Artists.AnyAsync(x => x.Id == artistId, ct))
        {
            return false;
        }

        var userId = await _appDbContext.GetCurrentUserProfileIdAsync(user, ct);

        _appDbContext.UserArtists.Add(new UserArtist
        {
            ArtistId = artistId,
            UserId = userId
        });

        await _appDbContext.SaveChangesAsync(ct);
        return true;
    }

    public async Task<bool> RemoveArtistAsync(long artistId, ClaimsPrincipal user, CancellationToken ct)
    {
        var userId = await _appDbContext.GetCurrentUserProfileIdAsync(user, ct);

        var artist = await _appDbContext.UserArtists
            .SingleOrDefaultAsync(x => x.UserId == userId && x.ArtistId == artistId, ct);

        if (artist == null)
        {
            return false;
        }

        _appDbContext.UserArtists.Remove(artist);

        await _appDbContext.SaveChangesAsync(ct);
        return true;
    }

    // Playlists

    public async Task<List<ShortGenrePlaylistDTO>> GetFavoritePlaylistsAsync(ClaimsPrincipal user, CancellationToken ct)
    {
        var userId = await _appDbContext.GetCurrentUserProfileIdAsync(user, ct);

        return await _appDbContext.Playlists
            .Where(x => x.UserPlaylists.Any(up => up.UserId == userId))
            .Select(x => new ShortGenrePlaylistDTO
            {
                CoverPath = x.CoverPath,
                Id = x.Id,
                Name = x.Name
            }).ToListAsync(ct);
    }

    public async Task<bool> AddPlaylistAsync(long playlistId, ClaimsPrincipal user, CancellationToken ct)
    {
        if (!await _appDbContext.Playlists.AnyAsync(x => x.Id == playlistId, ct))
        {
            return false;
        }

        var userId = await _appDbContext.GetCurrentUserProfileIdAsync(user, ct);

        _appDbContext.UserPlaylists.Add(new UserPlaylist
        {
            PlaylistId = playlistId,
            UserId = userId
        });

        await _appDbContext.SaveChangesAsync(ct);
        return true;
    }

    public async Task<bool> RemovePlaylistAsync(long playlistId, ClaimsPrincipal user, CancellationToken ct)
    {
        var userId = await _appDbContext.GetCurrentUserProfileIdAsync(user, ct);

        var playlist = await _appDbContext.UserPlaylists
            .SingleOrDefaultAsync(x => x.UserId == userId && x.PlaylistId == playlistId, ct);

        if (playlist == null)
        {
            return false;
        }

        _appDbContext.UserPlaylists.Remove(playlist);

        await _appDbContext.SaveChangesAsync(ct);
        return true;
    }
}
