using Audio_player.DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace Audio_player.DAL;

public class AppDbContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<AudioFile> AudioFiles { get; set; }
    public DbSet<AppRole> AppRoles { get; set; }
    public DbSet<AppUser> AppUsers { get; set; }
    public DbSet<AppUserRole> AppUserRoles { get; set; }
    public DbSet<UserProfile> UserProfiles { get; set; }
    public DbSet<Song> Songs { get; set; }
    public DbSet<UserSongs> UserSongs { get; set; }
    public DbSet<Playlist> Playlists { get; set; }
    public DbSet<UserPlaylist> UserPlaylists { get; set; }
    public DbSet<Album> Albums { get; set; }
    public DbSet<UserAlbum> UserAlbums { get; set; }
    public DbSet<Artist> Artists { get; set; }
    public DbSet<ArtistAlbum> ArtistAlbums { get; set; }
    public DbSet<ArtistSong> ArtistSongs { get; set; }
    public DbSet<UserArtist> UserArtists { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
    }
}
