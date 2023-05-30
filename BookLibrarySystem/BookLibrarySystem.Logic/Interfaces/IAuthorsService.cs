using BookLibrarySystem.Data.Models;

namespace BookLibrarySystem.Logic.Interfaces
{
    public interface IAuthorsService
    {
        Task<bool> CheckExistsAsync(int id);

        Task<IEnumerable<Author>> GetAuthorsAsync();

        Task<Author?> GetAuthorAsync(int id);

        Task<Author> AddAuthorAsync(Author author);

        Task<bool> UpdateAuthorAsync(Author author);

        Task<bool> DeleteAuthorAsync(int id);
    }
}
