using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Audio_player.DAL.Models;

public class UserAlbum
{
    public long UserId { get; set; }
    public long AlbumId { get; set; }
    public UserProfile? User {  get; set; }
    public Album? Album { get; set; }
}

public class UserAlbumConfig : IEntityTypeConfiguration<UserAlbum>
{
    public void Configure(EntityTypeBuilder<UserAlbum> builder)
    {
        builder.HasKey(x => new {x.UserId, x.AlbumId });
    }
}
