using BookLibrarySystem.Data;
using BookLibrarySystem.Data.Models;
using BookLibrarySystem.Logic.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BookLibrarySystem.Logic.Services
{
    public class LibraryService : ILibraryService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ILogger<LibraryService> _logger;   

        public LibraryService(ApplicationDbContext dbContext, ILogger<LibraryService> logger) 
        { 
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<IEnumerable<Book>> GetBooksAsync()
        {
            return await _dbContext.Books.Select(b=> 
                new Book{ Id = b.Id, 
                          Title= b.Title, 
                          Genre = b.Genre, 
                          Authors = b.BookAuthors.Select(a => a.Author).ToList()}).ToListAsync();
        }

        public async Task<Book?> GetBookAsync(int id)
        {
            try
            {
                return await _dbContext.Books.FindAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return null;
            }
        }

        public async Task<IEnumerable<Book>> SearchBooksAsync(string keyword)
        {
            // Perform the search across all fields using LINQ
            var matchedBooks = _dbContext.Books.Where(book =>
                book.Title.ToLower().Contains(keyword.ToLower()) ||
                book.BookAuthors.Any(ba => ba.Author.Name.ToLower().Contains(keyword.ToLower())) || //, StringComparison.OrdinalIgnoreCase
                book.Genre.ToLower().Contains(keyword.ToLower()) ||
                book.Publisher.ToLower().Contains(keyword.ToLower())  //, StringComparison.OrdinalIgnoreCase
            ).ToList();
            _logger.LogInformation("Doing Tests...");
            return matchedBooks;
        }

        public async Task<Book?> AddBookAsync(Book book)
        {
            try
            {
                await _dbContext.Books.AddAsync(book);
                await _dbContext.SaveChangesAsync();
                return await _dbContext.Books.FindAsync(book.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,$"The book {book.Title} could not be added.");
                return null;
            }
        }

        public async Task<Book> UpdateBookAsync(int id, Book book)
        {
            try
            {
                _dbContext.Entry(book).State = EntityState.Modified;
                await _dbContext.SaveChangesAsync();
                return book;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"The book {book.Title} could not be updated.");
                return null;
            }
        }

        public async Task<(bool, string)> DeleteBookAsync(Book book)
        {
            try
            {
                var dbBook = await _dbContext.Books.FindAsync(book.Id);

                if (dbBook == null)
                {
                    return (false, "The book could not be added.");
                }

                _dbContext.Books.Remove(book);
                await _dbContext.SaveChangesAsync();
                return (true, "Book got deleted");
            }
            catch (Exception ex)
            {
                return (false, $"An error occurred. Error message: {ex.Message}");
            }
        }

        //borrow
        //return 
        //edit
        //delete
    }
}
