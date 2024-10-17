using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Audio_player.DAL.Models;

public class UserArtist
{
    public long UserId { get; set; }
    public long ArtistId { get; set; }
    public UserProfile? User {  get; set; }
    public Artist? Artist { get; set; }
}

public class UserArtistConfig : IEntityTypeConfiguration<UserArtist>
{
    public void Configure(EntityTypeBuilder<UserArtist> builder)
    {
        builder.HasKey(x => new { x.UserId, x.ArtistId });
    }
}
