using BookLibrarySystem.Data.Models;
using BookLibrarySystem.Logic.DTOs;

namespace BookLibrarySystem.Logic.Interfaces
{
    public interface IUserService
    {
        Task<ApplicationUser> GetByUsernameAsync(string username);

        Task<ApplicationUser> GetByIdAsync(string Id);

        Task<IEnumerable<UserDto>> GetUsersAsync();

        Task<bool> CheckUserExistsAsync(string id);

        Task<ApplicationUser> AddUserAsync(UserDto newUser);   

        Task<bool> UpdateUserAsync(string id, UserDto modifiedUser);

        Task<bool> DeleteUserAsync(string id);

        Task<bool> BlockUserAsync(string userId);
    }
}
