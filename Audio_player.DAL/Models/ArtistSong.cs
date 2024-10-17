using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Audio_player.DAL.Models;

public class ArtistSong
{
    public long ArtistId { get; set; }
    public long SongId { get; set; }
    public Artist? Artist { get; set; }
    public Song? Song { get; set; }
}

public class ArtistSongConfig : IEntityTypeConfiguration<ArtistSong>
{
    public void Configure(EntityTypeBuilder<ArtistSong> builder)
    {
        builder.HasKey(x => new { x.SongId, x.ArtistId });
    }
}
