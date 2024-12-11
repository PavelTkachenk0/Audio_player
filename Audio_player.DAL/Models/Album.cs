using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Audio_player.DAL.Models;

public class Album : BaseEntity<long>
{
    public string AlbumName { get; set; } = null!;
    public string CoverPath { get; set; } = null!;
    public ICollection<Song> Songs { get; set; } = new HashSet<Song>();
    public ICollection<UserProfile> Users { get; set; } = new HashSet<UserProfile>();
    public ICollection<UserAlbum> UserAlbums { get; set; } = new HashSet<UserAlbum>();
    public ICollection<Artist> Artists { get; set; } = new HashSet<Artist>();
    public ICollection<ArtistAlbum> ArtistsAlbums { get; set;} = new HashSet<ArtistAlbum>();
    public ICollection<Genre> Genres { get; set; } = new HashSet<Genre>();
    public ICollection<GenreAlbum> GenreAlbums { get; set; } = new HashSet<GenreAlbum>();
}

public class AlbumConfig : IEntityTypeConfiguration<Album>
{
    public void Configure(EntityTypeBuilder<Album> builder)
    {
        builder.Property(x => x.AlbumName).HasMaxLength(100);
        builder.HasMany(x => x.Songs)
            .WithOne(x => x.Album)
            .HasForeignKey(x => x.AlbumId);
    }
}
