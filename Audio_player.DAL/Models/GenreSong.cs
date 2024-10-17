using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Audio_player.DAL.Models;

public class GenreSong 
{
    public short GenreId { get; set; }
    public long SongId { get; set; }
    public Genre? Genre { get; set; }
    public Song? Song { get; set; }
}

public class GenreSongConfig : IEntityTypeConfiguration<GenreSong>
{
    public void Configure(EntityTypeBuilder<GenreSong> builder)
    {
        builder.HasKey(x => new {x.SongId, x.GenreId});
    }
}
