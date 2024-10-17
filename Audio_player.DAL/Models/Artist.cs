using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Audio_player.DAL.Models;

public class Artist : BaseEntity<long>
{
    public string ArtistName { get; set; } = null!;
    public string CoverPath { get; set; } = null!;
    public ICollection<Album> Albums { get; set; } = new HashSet<Album>();
    public ICollection<ArtistAlbum> ArtistAlbums { get; set; } = new HashSet<ArtistAlbum>();
    public ICollection<Song> Songs { get; set; } = new HashSet<Song>();
    public ICollection<ArtistSong> ArtistSongs { get; set;} = new HashSet<ArtistSong>();
    public ICollection<UserProfile> Users { get; set; } = new HashSet<UserProfile>();
    public ICollection<UserArtist> UserArtists { get; set; } = new HashSet<UserArtist>();
    public ICollection<Genre> Genres { get; set; } = new HashSet<Genre>();
    public ICollection<GenreArtist> GenreArtists { get; set; } = new HashSet<GenreArtist>();
}

public class ArtistConfig : IEntityTypeConfiguration<Artist>
{
    public void Configure(EntityTypeBuilder<Artist> builder)
    {
        builder.Property(x => x.ArtistName).HasMaxLength(100);

        builder.HasMany(x => x.Albums)
            .WithMany(x => x.Artists)
            .UsingEntity<ArtistAlbum>(
                artistAlbum => artistAlbum
                    .HasOne(x => x.Album)
                    .WithMany(x => x.ArtistsAlbums)
                    .HasForeignKey(x => x.AlbumId),
                artistAlbum => artistAlbum
                    .HasOne(x => x.Artist)
                    .WithMany(x => x.ArtistAlbums)
                    .HasForeignKey(x => x.ArtistId)
            );

        builder.HasMany(x => x.Songs)
            .WithMany(x => x.Artists)
            .UsingEntity<ArtistSong>(
                artistSong => artistSong
                    .HasOne(x => x.Song)
                    .WithMany(x => x.ArtistSongs)
                    .HasForeignKey(x => x.SongId),
                artistSong => artistSong
                    .HasOne(x => x.Artist)
                    .WithMany(x => x.ArtistSongs)
                    .HasForeignKey(x => x.ArtistId)
            );
    }
}
