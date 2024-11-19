namespace Audio_player.AppSettingsOptions;

public class JobsSettingsOptions
{
    public CleanAccessTokensTableJobSettings CleanAccessTokensTableJobSettings { get; set; } = null!;
    public UpdateAccessTokensTableJobSettings UpdateAccessTokensTableJobSettings { get; set; } = null!;
    public UpdateRefreshTokensTableJobSettings UpdateRefreshTokensTableJobSettings { get; set; } = null!;
}

public class CleanAccessTokensTableJobSettings
{
    public string CronScheduler { get; set; } = null!;
}

public class UpdateAccessTokensTableJobSettings
{
    public string CronScheduler { set; get; } = null!;
}

public class UpdateRefreshTokensTableJobSettings
{
    public string CronScheduler { set; get; } = null!;
}