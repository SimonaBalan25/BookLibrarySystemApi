using BookLibrarySystem.Data;
using BookLibrarySystem.Data.Models;
using BookLibrarySystem.Logic.DTOs;
using BookLibrarySystem.Logic.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;

namespace BookLibrarySystem.Logic.Services
{
    public class BooksService : IBooksService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ILogger<BooksService> _logger;   
        private readonly IAuthorsService _authorsService;

        public BooksService(ApplicationDbContext dbContext, ILogger<BooksService> logger, IAuthorsService authorsService) 
        { 
            _dbContext = dbContext;
            _logger = logger;
            _authorsService = authorsService;   
        }

        public async Task<IEnumerable<BookDto>> GetBooksAsync()
        {
            _logger.LogDebug("Inside BooksService: GetBooksAsync method");
            
            return await _dbContext.Books.Select(b =>
                new BookDto
                {
                    Id = b.Id,
                    Title = b.Title,
                    Genre = b.Genre,
                    ISBN = b.ISBN,
                    NumberOfCopies = b.NumberOfCopies,  
                    LoanedQuantity = b.LoanedQuantity,
                    NumberOfPages = b.NumberOfPages,
                    Publisher = b.Publisher,
                    ReleaseYear = b.ReleaseYear,    
                    //Loans = b.Loans.ToList(),
                    //Reservations = b.Reservations.ToList(),  
                    //WaitingList = b.WaitingList.ToList(),
                    Authors = b.Authors.Select(a=>a.Id).ToList() 
                }).ToListAsync();
        }

        public async Task<BookDto?> GetBookAsync(int id)
        {
            try
            {
                return await _dbContext.Books.Select(b => new BookDto 
                { 
                    Id = b.Id,
                    Title = b.Title,
                    Genre = b.Genre,
                    ISBN= b.ISBN,   
                    LoanedQuantity= b.LoanedQuantity,
                    NumberOfCopies= b.NumberOfCopies,
                    NumberOfPages = b.NumberOfPages,
                    Publisher = b.Publisher,    
                    ReleaseYear = b.ReleaseYear,
                    Status = (int)b.Status,
                    Authors = b.Authors.Select(a=>a.Id)
                }).FirstOrDefaultAsync(b => b.Id.Equals(id));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"GetBookAsync(id): {ex.Message}");
                throw;
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

        public async Task<Book?> AddBookAsync(Book book, IEnumerable<int> authorIds)
        {
            try
            {               
                await _dbContext.Books.AddAsync(book);
                _dbContext.Entry(book).State = EntityState.Added;
                await _dbContext.SaveChangesAsync();

                foreach (var authorId in authorIds)
                {
                    var author = await _authorsService.GetAuthorAsync(authorId);
                    if (author != null)
                    {
                        var bookAuthor = new BookAuthor
                        {
                            BookId = book.Id,
                            AuthorId = author.Id
                        };
                        _dbContext.BookAuthors.Add(bookAuthor);
                    }
                }
                await _dbContext.SaveChangesAsync();

                return await _dbContext.Books.FindAsync(book.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,$"AddBookAsync method: Exception when trying to add the book {book.Title}.");
                throw;
            }
        }

        public async Task<bool> CheckExistsAsync(int bookId)
        {
            return await _dbContext.Books.FindAsync(bookId) != null;
        }

        public bool CanReserve(Book book)
        {
            return book.Status != BookStatus.Lost;
        }

        public bool CanBorrow(int bookId, string appUserId)
        {
            var bookToBorrow = _dbContext.Books.Find(bookId);
            var bookReservations = _dbContext.Reservations.Where(r => r.BookId.Equals(bookToBorrow.Id) && r.Status.Equals(ReservationStatus.Active)).Count();
            return bookToBorrow.LoanedQuantity + bookReservations < bookToBorrow.NumberOfCopies;
        }

        public async Task<int> BorrowBookAsync(int bookId, string userId)
        {
            var selectedBook = await _dbContext.Books.FindAsync(bookId);
            using (var dbTransaction = await _dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    selectedBook.LoanedQuantity += 1;
                    _dbContext.Books.Update(selectedBook);
                    _dbContext.Loans.Add(new BookLoan() { ApplicationUserId = userId, BookId = selectedBook.Id, BorrowedDate = DateTime.UtcNow, DueDate = DateTime.Now.AddDays(14), ReturnedDate = null, Renewed = false });

                    var result = await _dbContext.SaveChangesAsync();
                    await dbTransaction.CommitAsync();
                    return result;
                }
                catch (Exception ex)
                {
                    _logger.LogError("BookService- BorrowBook method: there was a problem in executing the method");
                    await dbTransaction.RollbackAsync();
                    throw;
                }
            }
        }

        public async Task<int> ReturnBookAsync(int bookId, string userId)
        {
            var selectedBook = await _dbContext.Books.FindAsync(bookId);
            using (var dbTransaction = await _dbContext.Database.BeginTransactionAsync()) 
            {
                try
                {
                    selectedBook.LoanedQuantity -= 1;
                    var loan = _dbContext.Loans.Where(l => l.ApplicationUserId == userId && l.BookId == selectedBook.Id).FirstOrDefault();
                    if (loan != null)
                    {
                        loan.ReturnedDate = DateTime.UtcNow;
                    }

                    //unblock the user if he is already blocked and he has no other loans that he hasn't returned

                    var result = await _dbContext.SaveChangesAsync();
                    await dbTransaction.CommitAsync();
                    return result;
                }
                catch (Exception ex)
                {
                    _logger.LogError("ReturnBook: There was a problem in running that method...");
                    await dbTransaction.RollbackAsync();
                    throw;
                }
            }
        }

        public async Task<bool> UpdateBookAsync(int bookId, BookDto selectedBook)
        {
            //use transactions in EF
            using (IDbContextTransaction transaction = await _dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    var dbBook = await _dbContext.Books.FindAsync(bookId);
                    dbBook.Genre = selectedBook.Genre;
                    dbBook.ISBN = selectedBook.ISBN;
                    dbBook.LoanedQuantity = selectedBook.LoanedQuantity;
                    dbBook.NumberOfPages = selectedBook.NumberOfPages;
                    dbBook.NumberOfCopies = selectedBook.NumberOfCopies;
                    dbBook.Publisher = selectedBook.Publisher;
                    dbBook.ReleaseYear = selectedBook.ReleaseYear;
                    dbBook.Title = selectedBook.Title;
                    //dbBook.BookAuthors = selectedBook.BookAuthors;
                    //dbBook.Loans = selectedBook.Loans;
                    //dbBook.Reservations = selectedBook.Reservations;
                    //dbBook.WaitingList = selectedBook.WaitingList;

                    _dbContext.Entry(dbBook).State = EntityState.Modified;
                    var result = await _dbContext.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return result > 0;
                }
                catch (Exception ex)
                {
                    _logger.LogError("UpdateBook: There was a database problem when trying to update..");
                    await transaction.RollbackAsync();
                    throw;
                }
            }
        }

        public async Task<bool> DeleteBookAsync(int bookId)
        {
            using (IDbContextTransaction transaction = _dbContext.Database.BeginTransaction())
            {
                try
                {
                    var dbBook = await _dbContext.Books.FindAsync(bookId);
                    var deleted = _dbContext.Books.Remove(dbBook);

                    _dbContext.Entry(dbBook).State = EntityState.Deleted;
                    var result = await _dbContext.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return result > 0;
                }
                catch(Exception ex)
                {
                    _logger.LogError("DeleteBook: There was a problem when trying to delete the book..");
                    await transaction.RollbackAsync();
                    throw;
                }
            }
        }

        public async Task<bool> ReserveBookAsync(int bookId, string appUserId)
        {
            using (IDbContextTransaction transaction = await _dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    await _dbContext.Reservations.AddAsync(new Reservation { ApplicationUserId = appUserId, BookId = bookId, ReservedDate = DateTime.Now, Status = ReservationStatus.Active });
                    var existingUsersInTheWaitingListForBook = _dbContext.WaitingList.Where(wl => wl.BookId.Equals(bookId)).ToList();
                    await _dbContext.WaitingList.AddAsync(new WaitingList { ApplicationUserId = appUserId, BookId = bookId, DateCreated = DateTime.Now, Position = existingUsersInTheWaitingListForBook.Count + 1 });
                    await _dbContext.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return true;
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }            
        }

        public async Task<bool> CancelReservationAsync(int bookId, string appUserId) 
        {
            try
            {
                var selectedBook = await _dbContext.Reservations.FirstOrDefaultAsync(r => r.BookId.Equals(bookId) && r.ApplicationUserId.Equals(appUserId));
                selectedBook.Status = ReservationStatus.Cancelled;
                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex) 
            {
                throw;
            }
        }

        public async Task<bool> RenewBookAsync(int bookId, string appUserId)
        {
            var dbLoan = await _dbContext.Loans.FirstOrDefaultAsync(loan => loan.BookId.Equals(bookId) && loan.ApplicationUserId.Equals(appUserId));
            
            dbLoan.Renewed = true;
            dbLoan.DueDate = DateTime.UtcNow.AddDays(10);
            dbLoan.Status = LoanStatus.Renewed;
            await _dbContext.SaveChangesAsync();
            return true;
        }

        private async Task<bool> CanBeRenewed(int bookId, string appUserId)
        {
            var dbLoan = await _dbContext.Loans.FirstOrDefaultAsync(loan => loan.ApplicationUserId.Equals(appUserId) && loan.BookId.Equals(bookId));
            var dbBook = await _dbContext.Books.FirstOrDefaultAsync(b => b.Id.Equals(bookId));
            var dbActiveReservations = await _dbContext.Reservations.Where(r => r.BookId.Equals(bookId) && r.Status.Equals(ReservationStatus.Active)).ToListAsync();

            return dbLoan != null && dbLoan.ReturnedDate == null && dbLoan.Status.Equals(LoanStatus.Active) && !dbLoan.Renewed && dbBook.NumberOfCopies - dbBook.LoanedQuantity > dbActiveReservations.Count();
        }
    }
}
