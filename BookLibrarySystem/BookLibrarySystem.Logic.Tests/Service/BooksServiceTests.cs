using AutoMapper;
using BookLibrarySystem.Data;
using BookLibrarySystem.Data.Models;
using BookLibrarySystem.Logic.Interfaces;
using BookLibrarySystem.Logic.Services;
using BookLibrarySystem.Logic.Tests.Factories;
using Duende.IdentityServer.EntityFramework.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Moq.EntityFrameworkCore;

namespace BookLibrarySystem.Logic.Tests.Service
{
    public class BooksServiceTests
    {
        public BooksServiceTests()
        {

        }

        private BooksService GetService(Mock<IAuthorsService> authorsServiceMock=null, Mock<ISendEmail> sendEmailServiceMock=null,
                Mock<ILogger<BooksService>> loggerMock=null, Mock<IMapper> mapperMock=null,
                Mock<ApplicationDbContext> dbContextMock=null)
        {
            if (authorsServiceMock == null)
            {
                authorsServiceMock = new Mock<IAuthorsService>();
            }

            if (sendEmailServiceMock == null)
            {
                sendEmailServiceMock = new Mock<ISendEmail>();
            }

            if (mapperMock == null)
            {
                mapperMock = new Mock<IMapper>();   
            }

            if (loggerMock == null)
            {
                loggerMock = new Mock<ILogger<BooksService>>(); 
            }

            if (dbContextMock == null)
            {
                var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                    .UseInMemoryDatabase(databaseName:"InMemoryAppDb") // Use in-memory database for testing
                    .Options;

                var operationalStoreOptions = Options.Create(new OperationalStoreOptions()
                {
                    // Configure options if necessary for your tests
                    // For example, you might want to set some default values that are used in production
                });

                dbContextMock = new Mock<ApplicationDbContext>(options, operationalStoreOptions);
                var myBooks = ReturnFakeBooksData();

                // Create a mock DbSet.
                var dbSet = MockDbSetFactory.Create(myBooks);

                // Set up the MyEntities property so it returns the mocked DbSet.
                dbContextMock.Setup(x => x.Books).ReturnsDbSet(dbSet.Object);
            }

            return new BooksService(dbContextMock.Object, loggerMock.Object, authorsServiceMock.Object, sendEmailServiceMock.Object, mapperMock.Object);
        }

        [Fact]
        public async Task GetAllAsync_ReturnsBooksList()
        {
            var booksService = GetService();

            var listResult = await booksService.GetBooksAsync();

            Assert.Equal(2, listResult.Count());
        }

        private IEnumerable<Book> ReturnFakeBooksData()
        {
            return new List<Book>()
            {
                new Book()
                {
                    Id = 1,
                    Title ="The lonely knight",
                    Genre="Fiction",
                    ISBN="135-221-272-1",
                    LoanedQuantity=1,
                    NumberOfCopies=3,
                    Publisher="De Sago",
                    ReleaseYear=2000,
                    Status=BookStatus.Available,
                    NumberOfPages=120
                },
                new Book()
                {
                    Id = 1,
                    Title ="Jerusalem city",
                    Genre="Documentary",
                    ISBN="135-221-272-2",
                    LoanedQuantity=1,
                    NumberOfCopies=3,
                    Publisher="De Sago",
                    ReleaseYear=2001,
                    Status=BookStatus.Available,
                    NumberOfPages=400
                }
            };
        }
    }
}