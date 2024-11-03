using Audio_player.DAL;
using Microsoft.EntityFrameworkCore;
using Quartz;

namespace Audio_player.Jobs;

public class UpdateRefreshTokensTableJob(AppDbContext appDbContext) : IJob
{
    private readonly AppDbContext _appDbContext = appDbContext;

    public async Task Execute(IJobExecutionContext context)
    {
        var tokens = _appDbContext.RefreshTokens.Where(x => x.ExpiryDate < DateTime.UtcNow && !x.IsRevoked);

        await tokens.ExecuteUpdateAsync(x => x.SetProperty(x => x.IsRevoked, true));
    }
}
