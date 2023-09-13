using BookLibrarySystem.Data.Models;
using BookLibrarySystem.Logic.DTOs;

namespace BookLibrarySystem.Logic.Interfaces
{
    public interface IAuthorsService
    {
        Task<bool> CheckExistsAsync(int id);

        Task<IEnumerable<AuthorDto>> GetAuthorsAsync();

        Task<AuthorDto?> GetAuthorAsync(int id);

        Task<Author> AddAuthorAsync(AuthorDto author);

        Task<bool> UpdateAuthorAsync(AuthorDto author);

        Task<bool> DeleteAuthorAsync(int id);
    }
}
