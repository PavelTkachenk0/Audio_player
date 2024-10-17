using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Audio_player.DAL.Models;

public class ArtistAlbum
{
    public long AlbumId { get; set; }
    public long ArtistId { get; set; }
    public Artist? Artist { get; set; }
    public Album? Album { get; set; }
}

public class ArtistAlbumConfig : IEntityTypeConfiguration<ArtistAlbum>
{
    public void Configure(EntityTypeBuilder<ArtistAlbum> builder)
    {
        builder.HasIndex(x => new {x.AlbumId, x.ArtistId });
    }
}
