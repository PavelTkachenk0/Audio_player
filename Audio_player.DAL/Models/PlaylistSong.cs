using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Audio_player.DAL.Models;

public class PlaylistSong 
{
    public long PlaylistId { get; set; }
    public long SongId { get; set; }
    public Song? Song { get; set; }
    public Playlist? Playlist { get; set; }
}

public class PlaylistSongConfig : IEntityTypeConfiguration<PlaylistSong>
{
    public void Configure(EntityTypeBuilder<PlaylistSong> builder)
    {
        builder.HasKey(x => new { x.PlaylistId, x.SongId });    
    }
}
