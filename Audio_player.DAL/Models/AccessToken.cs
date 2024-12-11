using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Audio_player.DAL.Models;

public class AccessToken : BaseEntity<long>
{
    public long UserId { get; set; }
    public string Jti { get; set; } = null!;
    public DateTime ExpiryDate { get; set; }
    public bool IsRevoked { get; set; }
    public AppUser User { get; set; } = null!;
}

public class AccessTokenConfig : IEntityTypeConfiguration<AccessToken>
{
    public void Configure(EntityTypeBuilder<AccessToken> builder)
    {
        builder.Property(x => x.Jti).HasMaxLength(256);
        builder.Property(x => x.IsRevoked).HasDefaultValue(false);
    }
}
