using Audio_player.DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace Audio_player.DAL;

public class AppDbContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<AudioFile> AudioFiles { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
    }
}
