namespace Audio_player.Helpers;

public class JobSynchronizationHelper
{
    private static readonly SemaphoreSlim accessTokenSemaphore = new(1, 1);

    public static async Task RunAccessTokensTableJobsWithLock(Func<Task> jobFunction)
    {
        await accessTokenSemaphore.WaitAsync();

        try
        {
            await jobFunction();
        }
        finally
        {
            accessTokenSemaphore.Release();
        }
    }
}
