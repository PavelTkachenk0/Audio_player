using Audio_player.DAL;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Audio_player.Extensions;

public static class UserClaimsExtensions
{
    /// <summary>
    /// Resolves the current user's UserProfile id from the NameIdentifier (email) claim.
    /// Returns 0 if the user cannot be found. Replaces the inline lookup that was
    /// duplicated across ~27 endpoints.
    /// </summary>
    public static async Task<long> GetCurrentUserProfileIdAsync(
        this AppDbContext dbContext, ClaimsPrincipal user, CancellationToken ct)
    {
        var email = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        return await dbContext.AppUsers
            .Where(x => x.Email == email)
            .Select(x => x.UserProfile!.Id)
            .SingleOrDefaultAsync(ct);
    }
}
