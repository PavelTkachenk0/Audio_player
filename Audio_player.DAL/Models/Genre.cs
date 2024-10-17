using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Audio_player.DAL.Models;

public class Genre : BaseEntity<short>
{
    public string Name { get; set; } = null!;
    public string CoverPath { get; set; } = null!;
    public ICollection<Song> Songs { get; set; } = new HashSet<Song>();
    public ICollection<GenreSong> GenreSongs { get; set; } = new HashSet<GenreSong>();
    public ICollection<Album> Albums { get; set; } = new HashSet<Album>();
    public ICollection<GenreAlbum> GenreAlbums { get; set; } = new HashSet<GenreAlbum>();
    public ICollection<Artist> Artists { get; set; } = new HashSet<Artist>();
    public ICollection<GenreArtist> GenreArtists { get; set; } = new HashSet<GenreArtist>();
}

public class GenreConfig : IEntityTypeConfiguration<Genre>
{
    public void Configure(EntityTypeBuilder<Genre> builder)
    {
        builder.Property(x => x.Name).HasMaxLength(100);

        builder.HasMany(x => x.Songs)
            .WithMany(x => x.Genres)
            .UsingEntity<GenreSong>(
                genreSong => genreSong
                    .HasOne(x => x.Song)
                    .WithMany(x => x.GenreSongs)
                    .HasForeignKey(x => x.SongId),
                genreSong => genreSong
                    .HasOne(x => x.Genre)
                    .WithMany(x => x.GenreSongs)
                    .HasForeignKey(x => x.GenreId)
            );

        builder.HasMany(x => x.Albums)
            .WithMany(x => x.Genres)
            .UsingEntity<GenreAlbum>(
                genreAlbum => genreAlbum
                    .HasOne(x => x.Album)
                    .WithMany(x => x.GenreAlbums)
                    .HasForeignKey(x => x.AlbumId),
                genreAlbum => genreAlbum
                    .HasOne(x => x.Genre)
                    .WithMany(x => x.GenreAlbums)
                    .HasForeignKey(x => x.GenreId)
            );

        builder.HasMany(x => x.Artists)
            .WithMany(x => x.Genres)
            .UsingEntity<GenreArtist>(
                genreArtist => genreArtist
                    .HasOne(x => x.Artist)
                    .WithMany(x => x.GenreArtists)
                    .HasForeignKey(x => x.ArtistId),
                genreArtist => genreArtist
                    .HasOne(x => x.Genre)
                    .WithMany(x => x.GenreArtists)
                    .HasForeignKey(x => x.GenreId)
            );

    }
}
