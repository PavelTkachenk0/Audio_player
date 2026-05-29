using Audio_player.DAL.Models;
using Audio_player.Models.DTOs;

namespace Audio_player.Mappers;

public static class UserMapper
{
    /// <summary>
    /// Server-side projection AppUser → UserDTO. Single source of truth shared by every
    /// user query; kept as an IQueryable extension so EF translates it to SQL.
    /// </summary>
    public static IQueryable<UserDTO> EntityToDto(this IQueryable<AppUser> users) =>
        users.Select(x => new UserDTO
        {
            Id = x.Id,
            Email = x.Email,
            Birthday = x.UserProfile!.Birthdate,
            Name = x.UserProfile!.Name,
            Surname = x.UserProfile!.Surname,
        });
}
