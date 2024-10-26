using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Audio_player.DAL.Models;

public class Playlist : BaseEntity<long>
{
    public string Name { get; set; } = null!;
    public string CoverPath { get; set; } = null!;
    public ICollection<UserProfile> Users { get; set; } = new HashSet<UserProfile>();
    public ICollection<UserPlaylist> UserPlaylists { get; set; } = new HashSet<UserPlaylist>();
    public ICollection<Song> Songs { get; set; } = new HashSet<Song>();
    public ICollection<PlaylistSong> PlaylistSongs { get; set; } = new HashSet<PlaylistSong>();
}

public class PlaylistConfig : IEntityTypeConfiguration<Playlist>
{
    public void Configure(EntityTypeBuilder<Playlist> builder)
    {
        builder.Property(x => x.Name).HasMaxLength(100);

        builder.HasMany(x => x.Songs)
            .WithMany(x => x.Playlists)
            .UsingEntity<PlaylistSong>(
                playlistSong => playlistSong
                    .HasOne(x => x.Song)
                    .WithMany(x => x.PlaylistSongs)
                    .HasForeignKey(x => x.SongId),
                playlistSong => playlistSong
                    .HasOne(x => x.Playlist)
                    .WithMany(x => x.PlaylistSongs)
                    .HasForeignKey(x => x.PlaylistId)
            );
    }
}
