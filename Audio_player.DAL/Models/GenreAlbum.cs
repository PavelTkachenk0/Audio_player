using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Audio_player.DAL.Models;

public class GenreAlbum 
{
    public long AlbumId { get; set; }
    public short GenreId { get; set; }
    public Genre? Genre { get; set; }
    public Album? Album { get; set; }
}

public class GenreAlbumConfig : IEntityTypeConfiguration<GenreAlbum>
{
    public void Configure(EntityTypeBuilder<GenreAlbum> builder)
    {
        builder.HasKey(x => new { x.AlbumId, x.GenreId });
    }
}
