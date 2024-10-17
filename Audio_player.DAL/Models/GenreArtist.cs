using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Audio_player.DAL.Models;

public class GenreArtist
{
    public short GenreId { get; set; }
    public long ArtistId { get; set; }
    public Genre? Genre { get; set; }
    public Artist? Artist { get; set; }
}

public class GenreArtistConfig : IEntityTypeConfiguration<GenreArtist>
{
    public void Configure(EntityTypeBuilder<GenreArtist> builder)
    {
        builder.HasKey(x => new { x.GenreId, x.ArtistId });
    }
}
