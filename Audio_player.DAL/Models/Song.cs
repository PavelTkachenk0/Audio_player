using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Audio_player.DAL.Models;

public class Song : BaseEntity<long>
{
    public string SongName { get; set; } = null!;
    public string SongPath { get; set; } = null!; 
    public long AlbumId { get; set; }
    public long ListeningCount { get; set; }
    public int Duration { get; set; }
    public ICollection<UserSongs> UserSongs { get; set; } = new HashSet<UserSongs>();
    public ICollection<UserProfile> Users { get; set; } = new HashSet<UserProfile>();
    public Album? Album { get; set; }
    public ICollection<Artist> Artists { get; set; } = new HashSet<Artist>();
    public ICollection<ArtistSong> ArtistSongs { get; set; } = new HashSet<ArtistSong>();
    public ICollection<Genre> Genres { get; set; } = new HashSet<Genre>();
    public ICollection<GenreSong> GenreSongs { get; set; } = new HashSet<GenreSong>();
    public ICollection<Playlist> Playlists { get; set; } = new HashSet<Playlist>();
    public ICollection<PlaylistSong> PlaylistSongs { get; set; } = new HashSet<PlaylistSong>();
}

public class SongConfig : IEntityTypeConfiguration<Song>
{
    public void Configure(EntityTypeBuilder<Song> builder)
    {
        builder.Property(x => x.SongName).HasMaxLength(100);
    }
}
