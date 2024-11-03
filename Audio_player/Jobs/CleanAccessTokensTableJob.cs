using Audio_player.DAL;
using Audio_player.Helpers;
using Quartz;

namespace Audio_player.Jobs;

public class CleanAccessTokensTableJob(AppDbContext appDbContext) : IJob
{
    private readonly AppDbContext _appDbContext = appDbContext;

    public async Task Execute(IJobExecutionContext context)
    {
        await JobSynchronizationHelper.RunAccessTokensTableJobsWithLock(async () =>
        {
            var tokens = _appDbContext.AccessTokens.Where(x => (DateTime.UtcNow - x.ExpiryDate).Days > 7);

            _appDbContext.AccessTokens.RemoveRange(tokens);

            await _appDbContext.SaveChangesAsync();
        });
    }
}
