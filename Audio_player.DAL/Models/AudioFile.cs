using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Audio_player.DAL.Models;

public class AudioFile : BaseEntity<long>
{
    public string FileName { get; set; } = null!;
    public string FilePath { get; set; } = null!;
}

public class AudioFileConfig : IEntityTypeConfiguration<AudioFile>
{
    public void Configure(EntityTypeBuilder<AudioFile> builder)
    {
        builder.Property(x => x.FileName)
            .HasMaxLength(256);

        builder.Property(x => x.FilePath)
            .HasMaxLength(256);

        builder.HasIndex(x => x.FileName);
    }
}