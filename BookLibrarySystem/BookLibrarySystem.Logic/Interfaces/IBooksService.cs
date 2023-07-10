using BookLibrarySystem.Data.Models;
using System.Threading.Tasks;


namespace BookLibrarySystem.Logic.Interfaces
{
    public interface IBooksService
    {
        Task<IEnumerable<Book>> GetBooksAsync();//get all books

        Task<Book?> GetBookAsync(int id);

        Task<IEnumerable<Book>> SearchBooksAsync(string keyword);

        Task<Book?> AddBookAsync(Book book, IEnumerable<int> authorIds);

        Task<int> BorrowBookAsync(Book book, string appUserId);

        bool ValidateBorrowAsync(Book selectedBook);

        Task<bool> CheckExistsAsync(int bookId);

        Task<int> ReturnBookAsync(Book book, string appUserId);

        Task<bool> UpdateBookAsync(int bookId, Book updatedBook);

        Task<bool> DeleteBookAsync(int bookId);

        
    }
}
