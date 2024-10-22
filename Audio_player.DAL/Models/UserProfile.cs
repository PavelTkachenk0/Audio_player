using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Query.Internal;

namespace Audio_player.DAL.Models;

public class UserProfile : BaseEntity<long>
{
    public string? Name { get; set; } = null!;
    public string? Surname { get; set; } 
    public DateTime? Birthdate { get; set; }
    public long AppUserId { get; set; }
    public AppUser? AppUser { get; set; }
    public ICollection<Song> Songs { get; set; } = new HashSet<Song>();
    public ICollection<UserSongs> UserSongs { get; set; } = new HashSet<UserSongs>();
    public ICollection<Album> Albums { get; set; } = new HashSet<Album>();
    public ICollection<UserAlbum> UserAlbums { get; set; } = new HashSet<UserAlbum>();
    public ICollection<Artist> Artists { get; set; } = new HashSet<Artist>();
    public ICollection<UserArtist> UserArtists { get; set; } = new HashSet<UserArtist>();
    public ICollection<Playlist> Playlists { get; set; } = new HashSet<Playlist>();
    public ICollection<UserPlaylist> UserPlaylists { get; set; } = new HashSet<UserPlaylist>();
}

public class UserProfileConfig : IEntityTypeConfiguration<UserProfile>
{
    public void Configure(EntityTypeBuilder<UserProfile> builder)
    {
        builder.Property(x => x.Name).HasMaxLength(100);
        builder.Property(x => x.Surname).HasMaxLength(100);

        builder.HasOne(x => x.AppUser)
            .WithOne(x => x.UserProfile)
            .HasForeignKey<UserProfile>(x => x.AppUserId);

        builder.HasMany(x => x.Songs)
            .WithMany(x => x.Users)
            .UsingEntity<UserSongs>(
                userSongs => userSongs
                    .HasOne(x => x.Song)
                    .WithMany(x => x.UserSongs)
                    .HasForeignKey(x => x.SongId),
                userSongs => userSongs
                    .HasOne(x => x.User)
                    .WithMany(x => x.UserSongs)
                    .HasForeignKey(x => x.UserId)
            );

        builder.HasMany(x => x.Albums)
            .WithMany(x => x.Users)
            .UsingEntity<UserAlbum>(
                userAlbum => userAlbum
                    .HasOne(x => x.Album)
                    .WithMany(x => x.UserAlbums)
                    .HasForeignKey(x => x.AlbumId),
                userAlbum => userAlbum
                    .HasOne(x => x.User)
                    .WithMany(x => x.UserAlbums)
                    .HasForeignKey(x => x.UserId)
            );

        builder.HasMany(x => x.Artists)
            .WithMany(x => x.Users)
            .UsingEntity<UserArtist>(
                userArtist => userArtist
                    .HasOne(x => x.Artist)
                    .WithMany(x => x.UserArtists)
                    .HasForeignKey(x => x.ArtistId),
                userArtist => userArtist
                    .HasOne(x => x.User)
                    .WithMany(x => x.UserArtists)
                    .HasForeignKey(x =>x.UserId)
            );

        builder.HasMany(x => x.Playlists)
            .WithMany(x => x.Users)
            .UsingEntity<UserPlaylist>(
                userPlaylist => userPlaylist
                    .HasOne(x => x.Playlist)
                    .WithMany(x => x.UserPlaylists)
                    .HasForeignKey(x => x.PlaylistId),
                userPlaylist => userPlaylist
                    .HasOne(x => x.User)
                    .WithMany(x => x.UserPlaylists)
                    .HasForeignKey(x => x.UserId)
            );
    }
}