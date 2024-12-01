using AutoMapper;
using BookLibrarySystem.Common.Models;
using BookLibrarySystem.Data;
using BookLibrarySystem.Data.Models;
using BookLibrarySystem.Logic.DTOs;
using BookLibrarySystem.Logic.Entities;
using BookLibrarySystem.Logic.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using BookLibrarySystem.Common.Extensions;
using System.Linq.Dynamic.Core;

namespace BookLibrarySystem.Logic.Services
{
    public class BooksService : IBooksService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ILogger<BooksService> _logger;   
        private readonly IAuthorsService _authorsService;
        private readonly ISendEmail _emailService;
        private readonly IMapper _mapper;
        private const int NoOfDaysForBookOnReturn = 10;
        private const int MaxNumberOfBooksToReserve = 3;
        private const int MaxNumberOfBooksToBorrow = 3;
        private const string DeleteReservationEmailSubject = "Deleted Reservation";
        private const string DeleteReservationEmailBody = "For book {0}, reservation will be deleted and also from the waiting list";

        public BooksService(ApplicationDbContext dbContext, ILogger<BooksService> logger, 
                IAuthorsService authorsService, ISendEmail emailService, IMapper mapper) 
        { 
            _dbContext = dbContext;
            _logger = logger;
            _authorsService = authorsService;
            _emailService = emailService;
            _mapper = mapper;
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

        public async Task<IEnumerable<BookForListing>> GetBooksForListingAsync()
        {
            return await _dbContext.Books.Select(b => new BookForListing
            {
                Title = b.Title,
                Id = b.Id
                //Publisher = b.Publisher,
                //ReleaseYear = b.ReleaseYear
            }).ToListAsync();
        }

        public async Task<IEnumerable<BookWithRelatedInfo>> GetBooksWithRelatedInfoAsync(string userId)
        {
            IList<BookWithRelatedInfo> result = null;

            try
            {
                result = await _dbContext.Books.GroupJoin(_dbContext.Reservations,
                    b => b.Id,
                    reservation => reservation.BookId,
                    (book, reservationGroup) => new BookWithRelatedInfo
                    {
                        Id = book.Id,
                        Title = book.Title,
                        Genre = book.Genre,
                        ISBN = book.ISBN,
                        ReleaseYear = book.ReleaseYear,
                        Publisher = book.Publisher,
                        Status = book.Status,
                        LoanedQuantity = book.LoanedQuantity,
                        NumberOfCopies = book.NumberOfCopies,
                        NumberOfPages = book.NumberOfPages,
                        Authors = book.Authors.Select(a => a.Id),
                        User = userId,
                        IsReservedByUser = reservationGroup.Any(r => r.ApplicationUserId == userId && r.Status.Equals(ReservationStatus.Active))
                    }).ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError("Exception when trying to get the books for a user");
            }
            return result;
        }

        public async Task<PagedResponse<BookDto>> GetBySearchFiltersAsync(string sortDirection, int pageIndex=1, int pageSize=10,  string sortColumn="", Dictionary<string,string> filters=null)
        {
            IQueryable<BookDto> filteredBooks = _dbContext.Books.Select(_mapper.Map<Book, BookDto>).AsQueryable();
            var totalCount = filteredBooks.Count();
            if (pageIndex >= 0 && pageSize > 0)
            {
                filteredBooks = filteredBooks.Skip(pageIndex * pageSize)
                                .Take(pageSize);
            }            

            if (filters != null)
            {
                string filterExpression;
                foreach (var filter in filters) 
                {
                    if (filter.Value != null)
                    {                       
                        string columnName = $"{filter.Key[0].ToString().ToUpper()}{filter.Key.Substring(1)}";
                        string filterValue = filter.Value;

                        bool isStringColumn = typeof(Book).GetProperty(columnName)?.PropertyType == typeof(string);
                        if (!isStringColumn)
                        {
                            filterExpression = $"{columnName} == @0";
                        }
                        else
                        {
                            filterExpression = $"{columnName}.Contains(@0)";
                        }

                        // Apply the filter using Dynamic LINQ
                        filteredBooks = filteredBooks.Where(filterExpression, filterValue);
                    }
                }
            }

            IOrderedQueryable<BookDto> orderedBooks = filteredBooks as IOrderedQueryable<BookDto>;
            if (!string.IsNullOrEmpty(sortColumn))
            {
                // Build the dynamic sorting expression
                var orderByExpression = $"{sortColumn} {sortDirection}";

                // Apply dynamic sorting
                orderedBooks = filteredBooks.OrderBy(orderByExpression);

            }
            
            return new PagedResponse<BookDto> { Rows = orderedBooks, TotalItems = totalCount };
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
                    Status = b.Status,
                    Version = b.Version,
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
                book.Authors.Any(a => a.Name.ToLower().Contains(keyword.ToLower())) || //, StringComparison.OrdinalIgnoreCase
                book.Genre.ToLower().Contains(keyword.ToLower()) ||
                book.Publisher.ToLower().Contains(keyword.ToLower())  //, StringComparison.OrdinalIgnoreCase
            ).ToList();
            _logger.LogInformation("Doing Tests...");
            return matchedBooks;
        }

        public async Task<bool> CheckExistsAsync(int bookId)
        {
            return await _dbContext.Books.FindAsync(bookId) != null;
        }

        public async Task<CanProcessBookResponse> CanReserveAsync(int bookId, string appUserId)
        {
            var dbUser = await _dbContext.Users.FindAsync(appUserId);
            if (dbUser.Status.Equals(UserStatus.Blocked))
            {
                return new CanProcessBookResponse { Allowed = false, Reason = "User is blocked, cannot make reservations !" };
            }

            var dbBook = await _dbContext.Books.FindAsync(bookId);
            if (dbBook.Status == BookStatus.Lost)
            {
                return new CanProcessBookResponse { Allowed = false, Reason = "The book is reported to be lost, cannot be reserved." };
            }

            var reservedBooksCount = _dbContext.Reservations.Where(r => r.ApplicationUserId.Equals(appUserId) && r.Status.Equals(ReservationStatus.Active)).Count();
            if (reservedBooksCount >= MaxNumberOfBooksToReserve)
            {
                return new CanProcessBookResponse { Allowed = false, Reason = "User cannot reserve any book, since he already has reserved 3 books." };
            }

            var checkIfSameBookIsLoaned = await _dbContext.Loans.Where(l=>l.ApplicationUserId.Equals(appUserId) && l.BookId.Equals(bookId)).FirstOrDefaultAsync() != null;
            if (checkIfSameBookIsLoaned)
            {
                return new CanProcessBookResponse { Allowed = false, Reason = "User has already loaned this book, he cannot reserve it again" };
            }

            return new CanProcessBookResponse { Allowed = true };
        }

        public async Task<CanProcessBookResponse> CanBorrowAsync(int bookId, string appUserId)
        {
            var bookToBorrow = await _dbContext.Books.FindAsync(bookId);
            var userWhoBorrows = await _dbContext.Users.FindAsync(appUserId);

            //check user status, if he is blocked, he cannot loan any book
            if (userWhoBorrows.Status == UserStatus.Blocked)
            {
                return new CanProcessBookResponse { Allowed = false, Reason = "User is set on blocked, he cannot do any loan !" };
            }

            //if the book is reported to be lost, then he cannot loan it
            if (bookToBorrow.Status == BookStatus.Lost)
            {
                return new CanProcessBookResponse { Allowed = false, Reason = "The book is reported to be lost." };
            }

            //if user already loaned it, he cannot loan it again
            if (_dbContext.Loans.Where(l => l.ApplicationUserId.Equals(appUserId) && l.BookId.Equals(bookId) && !l.Status.Equals(LoanStatus.Finalized)).Count() > 0)
            {
                return new CanProcessBookResponse { Allowed = false, Reason = "The book is already borrowed by this user, he cannot borrow it again !" };
            }

            //if there aren't enough copies, user cannot borrow that book
            var bookReservations = _dbContext.Reservations.Where(r => r.BookId.Equals(bookToBorrow.Id) && r.Status.Equals(ReservationStatus.Active)).Count();
            var enoughCopies = bookToBorrow.LoanedQuantity + bookReservations < bookToBorrow.NumberOfCopies;
            if ( !enoughCopies )
            {
                return new CanProcessBookResponse { Allowed = false, Reason = "Book cannot be borrowed, since all the copies are already given." };
            }

            //if user has already 3 books borrowed, he cannot borrow anymore until he returns 1 at least
            var booksBorrowedByUser = _dbContext.Loans.Where(l => l.ApplicationUserId.Equals(appUserId) && l.Status.Equals(LoanStatus.Active)).Select(l2=>l2.BookId).ToList();
            if (booksBorrowedByUser.Count >= MaxNumberOfBooksToBorrow)
            {
                return new CanProcessBookResponse { Allowed = false, Reason = "User has already 3 books borrowed, it's the maximum number in a certain time." };
            }

            return new CanProcessBookResponse { Allowed = true };
        }

        public async Task<bool> CanRenewAsync(int bookId, string appUserId)
        {
            var dbLoan = await _dbContext.Loans.FirstOrDefaultAsync(loan => loan.ApplicationUserId.Equals(appUserId) && loan.BookId.Equals(bookId) && loan.Status.Equals(LoanStatus.Active));
            var dbBook = await _dbContext.Books.FirstOrDefaultAsync(b => b.Id.Equals(bookId));
            
            //check there are available book copies
            var dbActiveReservations = await _dbContext.Reservations.Where(r => r.BookId.Equals(bookId) && r.Status.Equals(ReservationStatus.Active)).ToListAsync();

            return dbLoan != null &&
                dbLoan.ReturnedDate == null &&
                dbBook?.NumberOfCopies - dbBook?.LoanedQuantity > dbActiveReservations.Count();
        }

        public async Task<CanProcessBookResponse> CanReturnAsync(int bookId, string appUserId)
        {
            var selectedLoan = await _dbContext.Loans.FirstOrDefaultAsync(loan => loan.BookId.Equals(bookId) && loan.ApplicationUserId.Equals(appUserId) && !loan.Status.Equals(LoanStatus.Finalized));
            return new CanProcessBookResponse() { Allowed = selectedLoan != null, Reason = "There is no loan registered with this book & user" };
        }

        public async Task<Book?> AddBookAsync(BookDto bookDto, IEnumerable<int> authorIds)
        {
            var dbBook = _mapper.Map<Book>(bookDto);
            dbBook.Id = 0;
            using (var dbTransaction = await _dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    await _dbContext.Books.AddAsync(dbBook);
                    _dbContext.Entry(dbBook).State = EntityState.Added;
                    await _dbContext.SaveChangesAsync();

                    foreach (var authorId in authorIds)
                    {
                        var author = await _authorsService.GetAuthorAsync(authorId);
                        if (author != null)
                        {
                            var bookAuthor = new BookAuthor
                            {
                                BookId = dbBook.Id,
                                AuthorId = author.Id
                            };
                            _dbContext.BookAuthors.Add(bookAuthor);
                        }
                    }
                    await _dbContext.SaveChangesAsync();
                    await dbTransaction.CommitAsync();
                    return await _dbContext.Books.FindAsync(dbBook.Id);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"AddBookAsync method: Exception when trying to add the book {dbBook.Title}.");
                    await dbTransaction.RollbackAsync();    
                    throw;
                }
            }
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

                    //add loan
                    _dbContext.Loans.Add(new BookLoan() { ApplicationUserId = userId, BookId = selectedBook.Id, BorrowedDate = DateTime.UtcNow, DueDate = DateTime.Now.AddDays(NoOfDaysForBookOnReturn), ReturnedDate = null, Status=LoanStatus.Active });

                    //update reservation status if exists
                    var reservation = await  _dbContext.Reservations.FirstOrDefaultAsync(r => r.BookId.Equals(bookId) && r.ApplicationUserId.Equals(userId) && r.Status.Equals(ReservationStatus.Active));                     //delete from the waiting list
                    if (reservation != null)
                    {
                        reservation.Status = ReservationStatus.Finalized;
                    }

                    //set in the waiting list position -1 + update the other positions
                    var waitingUser = await _dbContext.WaitingList.FirstOrDefaultAsync(wl => wl.BookId.Equals(bookId) && wl.ApplicationUserId.Equals(userId));
                    if (waitingUser != null)
                    {
                        //_dbContext.WaitingList.Remove(waitingUser);
                        waitingUser.Position = -1;
                    }

                    var existingUsersInTheWaitingList = await _dbContext.WaitingList.Where(wl=>wl.BookId.Equals(bookId)).ToListAsync();
                    foreach(var user in existingUsersInTheWaitingList) 
                    {
                        user.Position -= 1;
                    }

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
                    //update loan status
                    var loan =  await _dbContext.Loans.FirstOrDefaultAsync(l => l.ApplicationUserId == userId && l.BookId == selectedBook.Id && !l.Status.Equals(LoanStatus.Finalized));
                    
                    loan.ReturnedDate = DateTime.UtcNow;
                    loan.Status = LoanStatus.Finalized;
                    selectedBook.LoanedQuantity -= 1;

                    //unblock the user if he is already blocked and he has no other loans that he hasn't returned
                    var selectedUser = await _dbContext.Users.FirstOrDefaultAsync(usr => usr.Id.Equals(userId));
                    if (selectedUser?.Status == UserStatus.Blocked)
                    {
                        var notReturnedBooks = await _dbContext.Loans.Where(l => l.ApplicationUserId.Equals(userId) && l.DueDate < DateTime.Now && l.Status.Equals(LoanStatus.Active)).ToListAsync();
                        if (notReturnedBooks.Count() == 0)
                        {
                            selectedUser.Status = UserStatus.Active;
                        }
                    }
                    
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

                    if (!ByteArraysEqual(dbBook.Version, selectedBook.Version))
                    {
                        throw new DbUpdateConcurrencyException("User is trying to update a version that doesn't exist anymore");
                    }

                    dbBook.Genre = selectedBook.Genre;
                    dbBook.ISBN = selectedBook.ISBN;
                    dbBook.LoanedQuantity = selectedBook.LoanedQuantity;
                    dbBook.NumberOfPages = selectedBook.NumberOfPages;
                    dbBook.NumberOfCopies = selectedBook.NumberOfCopies;
                    dbBook.Publisher = selectedBook.Publisher;
                    dbBook.ReleaseYear = selectedBook.ReleaseYear;
                    dbBook.Title = selectedBook.Title;
                    // Update the timestamp
                    dbBook.Version = BitConverter.GetBytes(DateTime.Now.Ticks);
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

        public async Task<CanProcessBookResponse> CanDeleteAsync(int bookId)
        {
            if (_dbContext.Loans.Any(l => l.BookId.Equals(bookId)))
            {
                return new CanProcessBookResponse { Allowed = false, Reason = "There are loans with this book" };
            }

            return new CanProcessBookResponse { Allowed = false };
        }

        public async Task<bool> DeleteBookAsync(int bookId)
        {
            using (IDbContextTransaction transaction = await _dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    var dbBook = await _dbContext.Books.FindAsync(bookId);
                    var deleted = _dbContext.Books.Remove(dbBook);

                    //delete authors, if they are only assigned to the book to delete
                    var selectedBookAuthors = await _dbContext.BookAuthors.Where(a => a.BookId == bookId).ToListAsync();
                    foreach (var bookAuthor in selectedBookAuthors)
                    {
                        if (_dbContext.BookAuthors.Any(ba => ba.BookId.Equals(bookId) && ba.AuthorId.Equals(bookAuthor.AuthorId)))
                        {
                            var selectedAuthor = await _dbContext.Authors.FindAsync(bookAuthor.AuthorId);
                            _dbContext.Authors.Remove(selectedAuthor);
                        }
                    }                    

                    _dbContext.Entry(dbBook).State = EntityState.Deleted;

                    IQueryable<Reservation> reservationsToDelete = _dbContext.Reservations.Where(r => r.BookId.Equals(bookId));
                    var usersToBeEmailed = reservationsToDelete.Select(r => r.ApplicationUserId).ToList();
                    var usersEmails = new List<string>();
                    usersToBeEmailed.ForEach(usr => usersEmails.Add(_dbContext.Users.Find(usr)?.Email));
                    await reservationsToDelete.ExecuteDeleteAsync();
                    await _dbContext.WaitingList.Where(wl => wl.BookId.Equals(bookId)).ExecuteDeleteAsync();

                    foreach (var user in usersEmails)
                    {
                        await _emailService.SendMailMessageAsync(user, DeleteReservationEmailSubject, string.Format(DeleteReservationEmailBody, dbBook.Title));
                    }
                    
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
                    //add record in Reservations table
                    await _dbContext.Reservations.AddAsync(new Reservation { ApplicationUserId = appUserId, BookId = bookId, ReservedDate = DateTime.Now, Status = ReservationStatus.Active });
                    //add record in the WaitingList table
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
            using (var dbTransaction = await _dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    var selectedReservation = await _dbContext.Reservations.FirstOrDefaultAsync(r => r.BookId.Equals(bookId) && r.ApplicationUserId.Equals(appUserId));
                    if (selectedReservation != null)
                    {
                        selectedReservation.Status = ReservationStatus.Cancelled;
                        await _dbContext.SaveChangesAsync();
                    }

                    //set records from the waiting list as Position on -1
                    var waitingListForCancelledReservation = await _dbContext.WaitingList.Where(wl => wl.BookId.Equals(bookId)).ToListAsync();
                    foreach (var waitingListRecord in waitingListForCancelledReservation)
                    {
                        waitingListRecord.Position = -1;
                    }
                    await _dbContext.SaveChangesAsync();
                    await dbTransaction.CommitAsync();
                    return true;
                }
                catch (Exception ex)
                {
                    _logger.LogError("Something happened when cancelling the reservation");
                    await dbTransaction.RollbackAsync();
                    throw;
                }
            }
        }

        public async Task<DateTime> RenewBookAsync(int bookId, string appUserId)
        {
            var dbLoan = await _dbContext.Loans.FirstOrDefaultAsync(loan => loan.BookId.Equals(bookId) && loan.ApplicationUserId.Equals(appUserId));

            if (dbLoan.Status != LoanStatus.Renewed)
            {
                dbLoan.DueDate = DateTime.UtcNow.AddDays(10);
                dbLoan.Status = LoanStatus.Renewed;
                await _dbContext.SaveChangesAsync();
            }
            return dbLoan.DueDate;
        }

        private bool ByteArraysEqual(byte[] a, byte[] b)
        {
            if (a == null && b == null)
                return true;

            if (a == null || b == null || a.Length != b.Length)
                return false;

            for (int i = 0; i < a.Length; i++)
            {
                if (a[i] != b[i])
                    return false;
            }

            return true;
        }
    }
}
