using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Audio_player.DAL.Models;

public class AppUserRole
{
    public long Id { get; set; }
    public long UserId { get; set; }
    public short RoleId { get; set; }
    public AppRole? AppRole { get; set; }
    public AppUser? AppUser { get; set; }
}

public class AppUserRoleConfig : IEntityTypeConfiguration<AppUserRole>
{
    public void Configure(EntityTypeBuilder<AppUserRole> builder)
    {
        builder.HasIndex(x => new { x.UserId, x.RoleId });
    }
}
