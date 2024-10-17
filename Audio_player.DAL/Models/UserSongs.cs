using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Audio_player.DAL.Models;

public class UserSongs 
{
    public long SongId { get; set; }
    public long UserId { get; set; }
    public Song? Song { get; set; }
    public UserProfile? User {  get; set; }
}

public class UserSongsConfig : IEntityTypeConfiguration<UserSongs>
{
    public void Configure(EntityTypeBuilder<UserSongs> builder)
    {
        builder.HasKey(x => new {x.SongId, x.UserId });
    }
}
