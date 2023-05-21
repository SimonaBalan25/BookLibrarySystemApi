using BookLibrarySystem.Data.Models;


namespace BookLibrarySystem.Logic.Interfaces
{
    public interface ILibraryService
    {
        Task<IEnumerable<Book>> GetBooksAsync();//get all books

        Task<Book?> GetBookAsync(int id);

        Task<IEnumerable<Book>> SearchBooksAsync(string keyword);

        Task<Book?> AddBookAsync(Book book);

        Task<Book> UpdateBookAsync(int id, Book book);

        Task<(bool, string)> DeleteBookAsync(Book book);
    }
}
