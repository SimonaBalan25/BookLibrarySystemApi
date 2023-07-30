using BookLibrarySystem.Data.Models;
using BookLibrarySystem.Logic.DTOs;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;


namespace BookLibrarySystem.Logic.Interfaces
{
    public interface IBooksService
    {
        Task<IEnumerable<BookDto>> GetBooksAsync();//get all books

        Task<BookDto?> GetBookAsync(int id);

        Task<IEnumerable<Book>> SearchBooksAsync(string keyword);

        Task<Book?> AddBookAsync(Book book, IEnumerable<int> authorIds);

        Task<int> BorrowBookAsync(int bookId, string appUserId);

        bool CanBorrow(int selectedBook, string appUserId);

        Task<bool> CheckExistsAsync(int bookId);

        Task<int> ReturnBookAsync(int bookId, string appUserId);

        Task<bool> UpdateBookAsync(int bookId, BookDto updatedBook);

        Task<bool> DeleteBookAsync(int bookId);

        Task<bool> ReserveBookAsync(int bookId, string appUserId);

        Task<bool> CancelReservationAsync(int bookId, string appUserId);

        Task<bool> RenewBookAsync(int bookId, string appUserId);
    }
}
