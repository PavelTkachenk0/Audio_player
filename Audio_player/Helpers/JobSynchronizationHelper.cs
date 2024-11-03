namespace Audio_player.Helpers;

public class JobSynchronizationHelper
{
    private static readonly SemaphoreSlim semaphore = new(1, 1);

    public static async Task RunWithLock(Func<Task> jobFunction)
    {
        await semaphore.WaitAsync();

        try
        {
            await jobFunction();
        }
        finally
        {
            semaphore.Release();
        }
    }
}
