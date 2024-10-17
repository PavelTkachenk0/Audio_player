using Audio_player.DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace Audio_player.DAL;

public class AppDbContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<AudioFile> AudioFiles { get; set; }
    public DbSet<AppRole> AppRoles { get; set; }
    public DbSet<AppUser> AppUsers { get; set; }
    public DbSet<AppUserRole> AppUserRoles { get; set; }
    public DbSet<UserProfile> UserProfiles { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
    }
}
