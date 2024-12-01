using BookLibrarySystem.Common.Models;
using BookLibrarySystem.Data.Models;
using BookLibrarySystem.Logic.DTOs;
using BookLibrarySystem.Logic.Entities;

namespace BookLibrarySystem.Logic.Interfaces
{
    public interface IBooksService
    {
        Task<IEnumerable<BookDto>> GetBooksAsync();//get all books

        Task<BookDto?> GetBookAsync(int id);

        Task<PagedResponse<BookDto>> GetBySearchFiltersAsync(string sortDirection, int pageIndex, int pageSize, string sortColumn, Dictionary<string,string> filters);

        Task<IEnumerable<BookForListing>> GetBooksForListingAsync();

        Task<IEnumerable<BookWithRelatedInfo>> GetBooksWithRelatedInfoAsync(string userId);

        Task<IEnumerable<Book>> SearchBooksAsync(string keyword);

        Task<bool> CheckExistsAsync(int bookId);

        Task<CanProcessBookResponse> CanBorrowAsync(int selectedBook, string appUserId);

        Task<CanProcessBookResponse> CanReturnAsync(int selectedBook, string appUserId);

        Task<CanProcessBookResponse> CanDeleteAsync(int bookId);

        Task<CanProcessBookResponse> CanReserveAsync(int bookId, string appUserId);

        Task<bool> CanRenewAsync(int bookId, string appUserId);

        Task<Book?> AddBookAsync(BookDto book, IEnumerable<int> authorIds);

        Task<int> BorrowBookAsync(int bookId, string appUserId);

        Task<int> ReturnBookAsync(int bookId, string appUserId);

        Task<bool> UpdateBookAsync(int bookId, BookDto updatedBook);

        Task<bool> DeleteBookAsync(int bookId);

        Task<bool> ReserveBookAsync(int bookId, string appUserId);

        Task<bool> CancelReservationAsync(int bookId, string appUserId);

        Task<DateTime> RenewBookAsync(int bookId, string appUserId);
    }
}
