using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Audio_player.DAL.Models;

public class AppUser : BaseEntity<long>
{
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!; 
    public ICollection<AppRole> Roles { get; set; } = new HashSet<AppRole>();
    public ICollection<AppUserRole> UserRoles { get; set; } = new HashSet<AppUserRole>();  
    public UserProfile? UserProfile { get; set; }
}

public class AppUserConfig : IEntityTypeConfiguration<AppUser>
{
    public void Configure(EntityTypeBuilder<AppUser> builder)
    {
        builder.Property(x => x.Email).HasMaxLength(255);
        builder.Property(x => x.Password).HasMaxLength(255);

        builder.HasMany(x => x.Roles)
            .WithMany(x => x.Users)
            .UsingEntity<AppUserRole>(
                userRole => userRole
                    .HasOne(x => x.AppRole)
                    .WithMany(x => x.UserRoles)
                    .HasForeignKey(x => x.RoleId),
                userRole => userRole
                    .HasOne(x => x.AppUser)
                    .WithMany(x => x.UserRoles)
                    .HasForeignKey(x => x.UserId)
            );
    }
}
