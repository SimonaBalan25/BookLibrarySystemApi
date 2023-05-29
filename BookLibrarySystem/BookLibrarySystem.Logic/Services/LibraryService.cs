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

        public async Task<bool> CheckExistsAsync(int bookId)
        {
            return await _dbContext.Books.FindAsync(bookId) != null;
        }

        public bool ValidateBorrowAsync(Book bookToBorrow)
        {
            if (bookToBorrow.LoanedQuantity == bookToBorrow.NumberOfCopies)
            {
                return false;
            }

            return true;
        }

        public async Task<int> BorrowBookAsync(Book selectedBook, string userId)
        {
            selectedBook.LoanedQuantity += 1;
            _dbContext.Books.Update(selectedBook);
            _dbContext.Loans.Add(new BookLoan() { ApplicationUserId = userId, BookId = selectedBook.Id, BorrowedDate = DateTime.UtcNow, DueDate = DateTime.Now.AddDays(21), ReturnedDate = null });

            return await _dbContext.SaveChangesAsync();
        }

        public async Task<int> ReturnBookAsync(Book selectedBook, string userId)
        {
            selectedBook.LoanedQuantity -= 1;
            var loan = _dbContext.Loans.Where(l => l.ApplicationUserId == userId && l.BookId == selectedBook.Id).FirstOrDefault();
            if (loan != null)
            {
                loan.ReturnedDate = DateTime.UtcNow;
                
            }

            //unblock the user if he is already blocked and he has no other loans that he hasn't returned

            return await _dbContext.SaveChangesAsync();
        }

        public async Task<bool> UpdateBookAsync(int bookId, Book selectedBook)
        {
            var dbBook = await GetBookAsync(bookId);

            dbBook.Genre = selectedBook.Genre;  
            dbBook.ISBN = selectedBook.ISBN;
            dbBook.LoanedQuantity = selectedBook.LoanedQuantity;
            dbBook.NumberOfPages = selectedBook.NumberOfPages;
            dbBook.NumberOfCopies = selectedBook.NumberOfCopies;    
            dbBook.Publisher = selectedBook.Publisher;
            dbBook.ReleaseYear = selectedBook.ReleaseYear;  
            dbBook.Title = selectedBook.Title;
            dbBook.BookAuthors = selectedBook.BookAuthors;
            dbBook.Loans = selectedBook.Loans;
            dbBook.Reservations = selectedBook.Reservations;    
            dbBook.WaitingList = selectedBook.WaitingList;

            _dbContext.Entry(dbBook).State = EntityState.Modified;
            return await _dbContext.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteBookAsync(int bookId)
        {
            var dbBook = await GetBookAsync(bookId);

            var deleted = _dbContext.Books.Remove(dbBook);

            return await _dbContext.SaveChangesAsync() > 0;
        }
    }
}
