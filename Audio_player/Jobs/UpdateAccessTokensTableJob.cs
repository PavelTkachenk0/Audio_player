using Audio_player.DAL;
using Audio_player.Helpers;
using Microsoft.EntityFrameworkCore;
using Quartz;

namespace Audio_player.Jobs;

public class UpdateAccessTokensTableJob(AppDbContext appDbContext) : IJob
{
    private readonly AppDbContext _appDbContext = appDbContext;

    public async Task Execute(IJobExecutionContext context)
    {
        await JobSynchronizationHelper.RunWithLock(async () =>
        {
            var tokens = _appDbContext.AccessTokens.Where(x => x.ExpiryDate < DateTime.UtcNow && !x.IsRevoked);

            await tokens.ExecuteUpdateAsync(x => x.SetProperty(x => x.IsRevoked, true));
        });
    }
}
