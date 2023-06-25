using BookLibrarySystem.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace BookLibrarySystem.Data.Data
{
    public class DbInitializer
    {
        private readonly ModelBuilder _modelBuilder;

        public DbInitializer(ModelBuilder builder)
        {
            _modelBuilder = builder;
        }

        public void Seed()
        {
            _modelBuilder.Entity<Author>()
                .HasData(
                new Author() { Id = 1, Name = "Haruki Murakami", Country = "Japan" },
                new Author() { Id = 2, Name = "Helle Helle", Country = "Denmark" },
                new Author() { Id = 3, Name = "Georges Simenon", Country = "Belgium" },
                new Author() { Id = 4, Name = "Martin Simon", Country = "Denmark" },
                new Author() { Id = 5, Name = "Avi Silberchatz", Country = "USA" },
                new Author() { Id = 6, Name = "Paul Auster", Country = "USA"});

            _modelBuilder.Entity<Book>().HasData(
                new Book() { Id = 1, Title = "Kafka on the shore", Genre = "Fiction-SF", ISBN = "978-606-123-1", LoanedQuantity = 0, NumberOfCopies = 3, NumberOfPages = 505, Publisher = "Klim", ReleaseYear = 2007 },
                new Book() { Id = 2, Title = "1Q84", Genre = "Fiction-Romance", ISBN = "093-184-732-2", LoanedQuantity = 0, NumberOfCopies = 4, NumberOfPages = 808, Publisher = "Klim", ReleaseYear = 2011 },
                new Book() { Id = 3, Title = "Rodby-Puttgarden", Genre = "Fiction-Thriller", LoanedQuantity = 0, ISBN = "731-847-427-0", NumberOfCopies = 3, Publisher = "Samleren", ReleaseYear = 2011 },
                new Book() { Id = 4, Title = "Maigret", Genre = "Fiction-Crime", ISBN = "743-263-482-8", LoanedQuantity = 0, NumberOfCopies = 5, NumberOfPages = 144, Publisher = "Lindhart op Linghorf", ReleaseYear = 2011 },
                new Book() { Id = 5, Title = "Database System Concenpts 6th Edition", Genre = "NonFiction-Textbook", ISBN = "943-921-813-0", LoanedQuantity = 0, NumberOfCopies = 10, NumberOfPages = 505, Publisher = "McGraw-Hill", ReleaseYear = 2010 },
                new Book() { Id = 7, Title = "Windows 8.1-Effectiv udden touch", ISBN= "453-263-283-4", Publisher = "Textmaster", ReleaseYear= 2014, Genre= "NonFiction-Guide", NumberOfCopies=5, LoanedQuantity=0, NumberOfPages=255 },
                new Book() { Id=8, Title= "The New York Triogy", Publisher= "Faber and Faber", ISBN= "253-273-284-9", ReleaseYear=1985, Genre="Fiction-Crime", NumberOfCopies=3, LoanedQuantity=0, NumberOfPages=458 }
                ); 

            _modelBuilder.Entity<BookAuthor>().HasData(
                new BookAuthor() { AuthorId = 1, BookId = 1 },
                new BookAuthor() { AuthorId = 1, BookId = 2 },
                new BookAuthor() { AuthorId = 2, BookId = 3 },
                new BookAuthor() { AuthorId = 2, BookId = 4 },
                new BookAuthor() { AuthorId = 3, BookId = 5 },
                new BookAuthor() { AuthorId=5,BookId=7},
                new BookAuthor() { AuthorId=6,BookId=8}
                );

            //modelBuilder.Entity<ApplicationUser>().HasData(
            //new ApplicationUser() { Id= "a4fa6f02-30bc-4658-a685-2b49d370f4e7",UserName="sule.altintas@gmail.com",AccessFailedCount=0,Address="street Red Carpet, no 60", BirthDate=new DateTime(2000,12,12), Email= "sule.altintas@gmail.com",EmailConfirmed=true,LockoutEnabled=true,Name="Sule Altintas",PhoneNumber="0740.198.093",PhoneNumberConfirmed=true,Status=UserStatus.Active,TwoFactorEnabled = false},
            //new ApplicationUser() { Id= "72565658-efd6-43bc-8bfc-c73a579bc355",UserName="fahad.sajad@gmail.com",AccessFailedCount=0, Address="str Marului, no 20", BirthDate = new DateTime(1990, 03, 10), Email = "fahad.sajad@gmail.com", EmailConfirmed = true, LockoutEnabled = true, Name = "Fahad Sajad", PhoneNumber = "0741.193.038", Status = UserStatus.Active, TwoFactorEnabled = false },
            //new ApplicationUser() { Id= "5f128a19-bc3e-47b2-8e42-ab6c700ef214", UserName="sebastian.sbarna@gmail.com", AccessFailedCount = 0, Address="str Revolutiei, no 18", BirthDate = new DateTime(1983, 10, 28), Email = "sebastian.sbarna@gmail.com", EmailConfirmed = true, LockoutEnabled = true, Name = "Sebastian Sbarna", PhoneNumber = "0752.291.202", PhoneNumberConfirmed=true, Status = UserStatus.Active, TwoFactorEnabled = false },
            //new ApplicationUser() { Id= "2886bdbe-af85-4458-bb62-af4e900bfa61", UserName="kare.jorgensen", AccessFailedCount= }
            //);

            _modelBuilder.Entity<BookLoan>().HasData(
                new BookLoan() { Id = 1, ApplicationUserId = "c72ac90e-ca57-4c2f-b04b-17409032aea8", BookId = 1, BorrowedDate = new DateTime(2023, 05, 10), DueDate = new DateTime(2023, 05, 31), ReturnedDate = null },
                new BookLoan() { Id = 2, ApplicationUserId = "c72ac90e-ca57-4c2f-b04b-17409032aea8", BookId = 2, BorrowedDate = new DateTime(2023, 05, 10), DueDate = new DateTime(2023, 05, 31), ReturnedDate = null }
                );

            _modelBuilder.Entity<Reservation>().HasData(
                new Reservation() { Id = 1, ApplicationUserId = "c72ac90e-ca57-4c2f-b04b-17409032aea8", BookId = 3, ReservedDate = new DateTime(2023, 05, 17), Status = ReservationStatus.Pending },
                new Reservation() { Id = 2, ApplicationUserId = "c72ac90e-ca57-4c2f-b04b-17409032aea8", BookId = 4, ReservedDate = new DateTime(2023, 05, 17), Status = ReservationStatus.Pending }
                );
        }
    }
}
