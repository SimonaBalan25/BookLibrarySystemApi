using BookLibrarySystem.Data.Models;

namespace BookLibrarySystem.Logic.Interfaces
{
    public interface IUserService
    {
        Task<ApplicationUser> GetByUsernameAsync(string username);

        Task<ApplicationUser> GetByIdAsync(string Id);

        Task<IEnumerable<ApplicationUser>> GetUsersAsync();

        Task<bool> CheckUserExistsAsync(string id);

        Task<ApplicationUser> AddUserAsync(ApplicationUser newUser);   

        Task<bool> UpdateUserAsync(string id, ApplicationUser modifiedUser);

        Task<bool> DeleteUserAsync(string id);
    }
}
