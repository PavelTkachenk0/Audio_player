using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Audio_player.DAL.Models;

public class UserPlaylist
{
    public long UserId { get; set; }
    public long PlaylistId { get; set; }
    public bool Own {  get; set; }
    public Playlist? Playlist { get; set; }
    public UserProfile? User {  get; set; }
}

public class UserPlaylistConfig : IEntityTypeConfiguration<UserPlaylist>
{
    public void Configure(EntityTypeBuilder<UserPlaylist> builder)
    {
        builder.HasKey(x => new { x.UserId, x.PlaylistId });

        builder.Property(x => x.Own).HasDefaultValue(false);
    }
}
