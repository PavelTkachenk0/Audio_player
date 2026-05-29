using Audio_player.DAL;
using Audio_player.Mappers;
using Audio_player.Models.DTOs;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Audio_player.Services;

public enum DeleteUserResult
{
    NotFound,
    SelfDeletion,
    Deleted
}

public class UserService(AppDbContext appDbContext)
{
    private readonly AppDbContext _appDbContext = appDbContext;

    public async Task<List<UserDTO>> GetAllAsync(CancellationToken ct)
    {
        return await _appDbContext.AppUsers
            .EntityToDto()
            .ToListAsync(ct);
    }

    public async Task<UserDTO?> GetByIdAsync(long id, CancellationToken ct)
    {
        return await _appDbContext.AppUsers
            .Where(x => x.Id == id)
            .EntityToDto()
            .SingleOrDefaultAsync(ct);
    }

    public async Task<UserDTO> GetCurrentAsync(ClaimsPrincipal user, CancellationToken ct)
    {
        var email = user.FindFirst(ClaimTypes.NameIdentifier)?.Value!;

        return await _appDbContext.AppUsers
            .EntityToDto()
            .SingleAsync(x => x.Email == email, ct);
    }

    public async Task<DeleteUserResult> DeleteAsync(long id, ClaimsPrincipal currentUser, CancellationToken ct)
    {
        var user = await _appDbContext.AppUsers.SingleOrDefaultAsync(x => x.Id == id, ct);

        if (user == null)
        {
            return DeleteUserResult.NotFound;
        }

        if (user.Email == currentUser.FindFirst(ClaimTypes.NameIdentifier)?.Value!)
        {
            return DeleteUserResult.SelfDeletion;
        }

        _appDbContext.AppUsers.Remove(user);
        await _appDbContext.SaveChangesAsync(ct);

        return DeleteUserResult.Deleted;
    }
}
