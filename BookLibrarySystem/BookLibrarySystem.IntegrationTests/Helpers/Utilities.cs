

using BookLibrarySystem.Data;
using BookLibrarySystem.Data.Models;
using BookLibrarySystem.Logic.DTOs;
using Microsoft.EntityFrameworkCore;

namespace BookLibrarySystem.IntegrationTests.Helpers
{
    public static class Utilities
    {
        #region snippet1
        public static void InitializeDbForTests(ApplicationDbContext context)
        {
            context.Books.RemoveRange(context.Books);
            context.SaveChanges();

            if (!context.Books.Any())
            {
                context.Books.AddRange(GetBooksRecords());
                context.SaveChanges();
            }                 
            
            var appUser1 = Guid.NewGuid().ToString();
            var appUser2 = Guid.NewGuid().ToString();   
            var bookLoans = new List<BookLoan>
            {
                new BookLoan { BookId = 1, Id = 1, ApplicationUserId = appUser1, BorrowedDate = DateTime.Now, DueDate = DateTime.Now.AddDays(14), Status = LoanStatus.Active },
                new BookLoan { BookId = 1, Id = 2, ApplicationUserId = appUser1, BorrowedDate = DateTime.Now, DueDate = DateTime.Now.AddDays(14), Status = LoanStatus.Active },
                new BookLoan { BookId = 3, Id = 3, ApplicationUserId = appUser2, BorrowedDate = DateTime.Now, DueDate = DateTime.Now.AddDays(14), Status = LoanStatus.Active }
            };
            context.Loans.AddRange(bookLoans); 
            context.SaveChanges();
        }

        public static void ReinitializeDbForTests(ApplicationDbContext db)
        {
            db.Books.RemoveRange(db.Books);
            InitializeDbForTests(db);
        }

        public static List<Book> GetBooksRecords()
        {
            return new List<Book>()
            {
                new Book(){ Title = "Expected Book Title 1", Id=1, ISBN="333-111-44-113",Version = new byte[] { 0x00, 0x01 }, Genre="Fiction", Publisher="Bentley" },
                new Book(){ Title = "Expected Book Title 2", Id=2, ISBN="447-298-939-191",Version = new byte[] { 0x00, 0x01 }, Genre="Reality", Publisher="CGM" },
                new Book(){ Title = "Expected Book Title 3", Id=3, ISBN="372-283-191-1",Version = new byte[] { 0x00, 0x01 }, Genre="SF", Publisher="Bentley" }
            };
        }

        public static void PopulateTestData(ApplicationDbContext dbContext)
        {
            dbContext.Books.Add(new Book
            {
                Id = 1,
                Genre = "Fiction",
                ISBN = "273-373-282-2",
                LoanedQuantity = 1,
                Publisher = "Bentley",
                NumberOfCopies = 2,
                NumberOfPages = 120,
                ReleaseYear = 1990,
                Status = Data.Models.BookStatus.Available,
                Title = "Winter in Strasbourg",
                Version = new byte[] { 0x00, 0x01 },
                Authors = new List<Author> { new Author { Id = 1, Name = "test author", Country = "Ireland" } }
            });
            //dbContext.Books.Add(new Book() { Title = "TEST RECORD: Would you like a jelly baby?", Id = 2, ISBN = "447-298-939-191", Version = new byte[] { 1,2,3,4 } });
            //dbContext.Books.Add(new Book() { Title = "TEST RECORD: To the rational mind, ", Id = 3, ISBN = "372-283-191-1", Version = new byte[] { 1,2,3,4 } });
            dbContext.SaveChanges();
        }
        #endregion
    }
}
