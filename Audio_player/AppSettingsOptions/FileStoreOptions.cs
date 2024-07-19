namespace Audio_player.AppSettingsOptions;

public class FileStoreOptions
{
    public string FilesPath { get; set; } = null!;
    public string[] AudioExtensions { get; set; } = null!;
    public int MaxSize { get; set; }
}
