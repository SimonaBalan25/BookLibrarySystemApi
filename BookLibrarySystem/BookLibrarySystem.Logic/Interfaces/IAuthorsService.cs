using BookLibrarySystem.Common.Models;
using BookLibrarySystem.Data.Models;
using BookLibrarySystem.Logic.DTOs;

namespace BookLibrarySystem.Logic.Interfaces
{
    public interface IAuthorsService
    {
        Task<bool> CheckExistsAsync(int id);

        Task<IEnumerable<AuthorDto>> GetAuthorsAsync();

        Task<PagedResponse<AuthorDto>> GetAuthorsBySortColumnAsync(string sortDirection, string sortColumn);

        Task<AuthorDto?> GetAuthorAsync(int id);

        Task<Author> AddAuthorAsync(AuthorDto author);

        Task<bool> UpdateAuthorAsync(AuthorDto author);

        Task<bool> DeleteAuthorAsync(int id);

        Task<bool> AssignBooksAsync(int id, string[] booksIds);
    }
}
