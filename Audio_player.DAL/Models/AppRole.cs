using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Audio_player.DAL.Models;

public class AppRole : BaseEntity<short>
{
    public string Name { get; set; } = null!;
    public ICollection<AppUser> Users { get; set; } = new HashSet<AppUser>();
    public ICollection<AppUserRole> UserRoles { get; set; } = new HashSet<AppUserRole>();
}

public class AppRoleConfig : IEntityTypeConfiguration<AppRole>
{
    public void Configure(EntityTypeBuilder<AppRole> builder)
    {
        builder.Property(x => x.Name).HasMaxLength(100);
    }
}