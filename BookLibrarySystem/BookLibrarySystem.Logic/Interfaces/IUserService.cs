using BookLibrarySystem.Data.Models;

namespace BookLibrarySystem.Logic.Interfaces
{
    public interface IUserService
    {
        Task<ApplicationUser> GetByUsernameAsync(string username);

        Task<ApplicationUser> GetByIdAsync(string Id);
    }
}
