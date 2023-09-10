using BookLibrarySystem.Data.Models;
using BookLibrarySystem.Logic.DTOs;
using BookLibrarySystem.Logic.Entities;

namespace BookLibrarySystem.Logic.Interfaces
{
    public interface IBooksService
    {
        Task<IEnumerable<BookDto>> GetBooksAsync();//get all books

        Task<BookDto?> GetBookAsync(int id);

        Task<IEnumerable<Book>> SearchBooksAsync(string keyword);

        Task<bool> CheckExistsAsync(int bookId);

        Task<CanGetBookResponse> CanBorrowAsync(int selectedBook, string appUserId);

        Task<CanGetBookResponse> CanReserveAsync(int bookId, string appUserId);

        Task<bool> CanRenewAsync(int bookId, string appUserId);

        Task<Book?> AddBookAsync(BookDto book, IEnumerable<int> authorIds);

        Task<int> BorrowBookAsync(int bookId, string appUserId, IEnumerable<int> borrowed);

        Task<int> ReturnBookAsync(int bookId, string appUserId);

        Task<bool> UpdateBookAsync(int bookId, BookDto updatedBook);

        Task<bool> DeleteBookAsync(int bookId);

        Task<bool> ReserveBookAsync(int bookId, string appUserId);

        Task<bool> CancelReservationAsync(int bookId, string appUserId);

        Task<DateTime> RenewBookAsync(int bookId, string appUserId);
    }
}
