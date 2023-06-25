using BookLibrarySystem.Data.Models;

namespace BookLibrarySystem.Logic.Interfaces
{
    public interface IUserService
    {
        Task<ApplicationUser> GetByUsernameAsync(string username);

        Task<ApplicationUser> GetByIdAsync(string Id);

        Task<IEnumerable<ApplicationUser>> GetUsersAsync();   

        Task<ApplicationUser> AddUserAsync(ApplicationUser newUser);   

        Task<bool> UpdateUserAsync(Guid id, ApplicationUser modifiedUser);

        Task<bool> DeleteUserAsync(Guid id);
    }
}
