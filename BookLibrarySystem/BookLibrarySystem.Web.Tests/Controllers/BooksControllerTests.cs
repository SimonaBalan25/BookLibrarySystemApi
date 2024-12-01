using BookLibrarySystem.Data.Models;
using BookLibrarySystem.Logic.DTOs;
using BookLibrarySystem.Logic.Entities;
using BookLibrarySystem.Logic.Interfaces;
using BookLibrarySystem.Web.Controllers;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace BookLibrarySystem.Web.Tests.Controllers
{
    public class BooksControllerTests
    {
        private readonly Mock<ITelemetryService> _logger;

        public BooksControllerTests()
        {
            _logger = new Mock<ITelemetryService>();
            _logger.Setup(s => s.TrackEvent(It.IsAny<string>(), It.IsAny<SeverityLevel>(), It.IsAny<IDictionary<string, string>>())).Callback((string name, SeverityLevel level, IDictionary<string,string> props) => { });
        }

        public BooksController GetController(Mock<IBooksService> mock = null)
        {
            if (mock == null) 
            { 
                 mock = new Mock<IBooksService>();
            }

            return new BooksController(mock.Object, _logger.Object);
        }

        [Fact]
        public async Task GetAllAsync_Returns200OkSuccess()
        {
            var booksList = GetBooksData();
            var mock = new Mock<IBooksService>();
            mock.Setup(m => m.GetBooksAsync()).ReturnsAsync(booksList);
            var controller = GetController(mock);

            //Act
            var result = await controller.GetAllAsync();
            var okResult = result as ObjectResult;

            //arrange
            Assert.NotNull(okResult);
            Assert.IsAssignableFrom<IList<BookDto>>(okResult.Value);    
            Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
            Assert.Equal(booksList.Count(), (okResult.Value as IList<BookDto>)?.Count());
        }


        [Fact]
        public async Task GetBookByIDAsync_Returns200OkSuccess()
        {
            //arrange
            var booksList = GetBooksData();
            var mock = new Mock<IBooksService>();
            mock.Setup(x => x.GetBookAsync(2)).ReturnsAsync(booksList.First());
            var controller = GetController(mock);

            //act
            var bookResult = await controller.GetBookAsync(2);
            Assert.IsAssignableFrom<OkObjectResult>(bookResult);
            var bookObj = bookResult as OkObjectResult;

            //assert
            Assert.NotNull(bookObj);
            Assert.Equal(booksList.First().Id, (bookObj.Value as BookDto).Id);
            Assert.Equal(booksList.First().Publisher, (bookObj.Value as BookDto).Publisher);
        }

        [Fact]
        public async Task GetBookByIdAsync_ReturnsNotFound()
        {
            var booksList = GetBooksData();
            var notExistingBookId = 100;
            var mock = new Mock<IBooksService>();
            var controller = GetController(mock);

            //act
            var bookResult = await controller.GetBookAsync(notExistingBookId);
            Assert.IsAssignableFrom<NotFoundResult>(bookResult);
            var notFoundResponse = bookResult as NotFoundResult;
            Assert.Equal(StatusCodes.Status404NotFound, notFoundResponse.StatusCode);
        }

        [Fact]
        public async Task AddBookAsync_Returns200OkSuccess()
        {
           //arrange
            var booksList = GetBooksData();
            var selectedBook = booksList.ElementAt(2);
            var mock = new Mock<IBooksService>();
            mock.Setup(x => x.AddBookAsync(booksList.ElementAt(2), new List<int>{2}))
                .ReturnsAsync(new Book 
                { 
                    Id = selectedBook.Id,
                    Title = selectedBook.Title,
                    Genre = selectedBook.Genre,
                    ISBN = selectedBook.ISBN,
                    LoanedQuantity = selectedBook.LoanedQuantity,
                    NumberOfPages = selectedBook.NumberOfPages,
                    NumberOfCopies = selectedBook.NumberOfCopies,
                    ReleaseYear = selectedBook.ReleaseYear,
                    Publisher = selectedBook.Publisher
                });
            var controller = GetController(mock);

            //act
            var result = await controller.AddBookAsync(booksList.ElementAt(2));

            //assert
            Assert.IsType<CreatedAtActionResult>(result);
            var createdBook = result as CreatedAtActionResult;
            Assert.NotNull(result);
            Assert.IsType<BookDto>(createdBook.Value);
            var dtoBook = createdBook.Value as BookDto;
            Assert.Equal(booksList.ElementAt(2).Id, dtoBook.Id);
            Assert.Equal(booksList.ElementAt(2).Title, dtoBook.Title);
            Assert.Equal(booksList.ElementAt(2).Genre, dtoBook.Genre);
        }

        [Fact]
        public async Task BorrowBookAsync_WhenBookNotExists_Returns404NotFound()
        {
            //arrange
            var booksList = GetBooksData();
            var appUserId = Guid.NewGuid().ToString();  
            var bookId = booksList.ElementAt(0).Id;
            var mock = new Mock<IBooksService>();
            mock.Setup(x => x.CheckExistsAsync(bookId)).Returns(Task.FromResult(false));

            var controller = GetController(mock);

            //act
            var result = await controller.BorrowBookAsync(bookId, appUserId);

            //assert
            Assert.IsAssignableFrom<NotFoundResult>(result);
            var objResult = result as NotFoundResult;
            Assert.Equal(StatusCodes.Status404NotFound, objResult.StatusCode);
        }

        [Fact]
        public async Task BorrowBookAsync_WhenBookCannotBorrow_Returns400BadRequest()
        {
            //arrange
            var booksList = GetBooksData();
            var appUserId = Guid.NewGuid().ToString();
            var bookId = booksList.ElementAt(0).Id;
            var mock = new Mock<IBooksService>();
            mock.Setup(x => x.CheckExistsAsync(bookId)).Returns(Task.FromResult(true));
            mock.Setup(x => x.CanBorrowAsync(bookId, appUserId)).ReturnsAsync(new CanProcessBookResponse { Allowed = false, Reason="reason" });

            var controller = GetController(mock);

            //act
            var result = await controller.BorrowBookAsync(bookId, appUserId);

            //assert
            var objResult = result as BadRequestObjectResult;
            Assert.IsAssignableFrom<BadRequestObjectResult>(objResult);
            Assert.Equal(StatusCodes.Status400BadRequest, objResult.StatusCode);
        }

        [Fact]
        public async Task BorrowBookAsync_WhenBookResultIsLessThan0_Returns500InternalServerError()
        {
            //arrange
            var booksList = GetBooksData();
            var appUserId = Guid.NewGuid().ToString();
            var bookId = booksList.ElementAt(0).Id;
            var mock = new Mock<IBooksService>();
            mock.Setup(x => x.CheckExistsAsync(bookId)).Returns(Task.FromResult(true));
            mock.Setup(x => x.CanBorrowAsync(bookId, appUserId)).ReturnsAsync(new CanProcessBookResponse { Allowed = true });
            mock.Setup(x => x.BorrowBookAsync(bookId, appUserId)).ReturnsAsync(-1);

            var controller = GetController(mock);

            //act
            var result = await controller.BorrowBookAsync(bookId, appUserId);

            //assert
            Assert.NotNull(result);
            var objResult = result as ObjectResult;
            Assert.Equal(StatusCodes.Status500InternalServerError, objResult.StatusCode);
        }

        [Fact]
        public async Task ReturnBookAsync_WhenBookIsNotFound_Returns404NotFound()
        {
            var bookId = GetBooksData().First().Id;
            var appUserId = Guid.NewGuid().ToString();
            var mock = new Mock<IBooksService>();
            mock.Setup(x => x.CheckExistsAsync(bookId)).ReturnsAsync(false);

            var controller = GetController(mock);

            //act
            var result = await controller.ReturnBookAsync(bookId, appUserId);

            Assert.IsAssignableFrom<NotFoundResult>(result);
            var objResult = result as NotFoundResult;
            Assert.Equal(StatusCodes.Status404NotFound, objResult.StatusCode);
        }

        [Fact]
        public async Task ReturnBook_WhenNotAllowed_Return400BadRequest()
        {
            var bookId = GetBooksData().First().Id;
            var appUserId = Guid.NewGuid().ToString();
            var mock = new Mock<IBooksService>();
            mock.Setup(x => x.CheckExistsAsync(bookId)).ReturnsAsync(true);
            mock.Setup(x => x.CanReturnAsync(bookId, appUserId)).ReturnsAsync(new CanProcessBookResponse() { Allowed = false,Reason = "no loan registered with this bookId !"});

            var controller = GetController(mock);

            var result = await controller.ReturnBookAsync(bookId, appUserId);

            Assert.IsAssignableFrom<BadRequestObjectResult>(result);
            var objResult = result as BadRequestObjectResult;
            Assert.Equal(StatusCodes.Status400BadRequest, objResult?.StatusCode);
        }

        //delete book unit tests..
        [Fact]
        public async Task DeleteBook_InvalidId_ReturnsBadRequest()
        {
            var selectedBookId = 0;

            var controller = GetController();

            var result = await controller.DeleteBookAsync(selectedBookId);

            Assert.IsAssignableFrom<BadRequestObjectResult>(result);
            Assert.Equal(400, (result as ObjectResult).StatusCode);
        }

        [Fact]
        public async Task DeleteBook_WhenBookNotExists_ReturnsNotFound()
        {
            var selectedBookId = 20;
            var mock = new Mock<IBooksService>();
            mock.Setup(x=>x.CheckExistsAsync(selectedBookId)).ReturnsAsync(false);

            var controller = GetController(mock);

            var result = await controller.DeleteBookAsync(selectedBookId);
            Assert.IsAssignableFrom<NotFoundResult>(result);
            Assert.Equal(404, (result as NotFoundResult).StatusCode);
        }

        [Fact]
        public async Task DeleteBook_WhenNotAllowed_ReturnBadRequest()
        {
            var selectedBookId = 10;

            var mock = new Mock<IBooksService>();
            mock.Setup(x => x.CheckExistsAsync(selectedBookId)).ReturnsAsync(true);
            mock.Setup(x => x.CanDeleteAsync(selectedBookId)).ReturnsAsync(new CanProcessBookResponse { Allowed=false,Reason="Book doesn't exist in the library!"});

            var controller = GetController(mock);
            var result = await controller.DeleteBookAsync(selectedBookId);

            Assert.IsAssignableFrom<BadRequestObjectResult>(result);
            Assert.Equal(400, (result as BadRequestObjectResult).StatusCode);
        }

        [Fact]
        public async Task DeleteBook_WhenConditionsAreMet_BookGetsDeleted()
        {
            var selectedBookId = 15;

            var mock = new Mock<IBooksService>();
            mock.Setup(x => x.CanDeleteAsync(selectedBookId)).ReturnsAsync(new CanProcessBookResponse { Allowed=true});
            mock.Setup(x => x.CheckExistsAsync(selectedBookId)).ReturnsAsync(true);
            mock.Setup(x => x.DeleteBookAsync(selectedBookId)).ReturnsAsync(true);

            var controller = GetController(mock);
            var result = await controller.DeleteBookAsync(selectedBookId);

            Assert.IsAssignableFrom<OkResult>(result);
            Assert.Equal(200, (result as OkResult).StatusCode); 
        }

        //update
        [Fact]
        public async Task UpdateBook_WhenInvalidId_ReturnsBadRequest()
        {
            var bookIdToUpdate = 0;
            var bookDto = new BookDto { Genre = "Fiction", Id = 2, LoanedQuantity = 2, ISBN = "377-383-292-1", NumberOfCopies = 1, NumberOfPages = 120, Publisher = "publisher", ReleaseYear = 2010, Status = BookStatus.Available, Title = "Mystery in Paris", Version = new byte[] { 1, 2, 3 } };
            var controller = GetController();

            //act
            var result = await controller.UpdateBookAsync(bookIdToUpdate, bookDto);

            Assert.IsAssignableFrom<BadRequestObjectResult>(result);
            Assert.Equal(StatusCodes.Status400BadRequest, (result as BadRequestObjectResult).StatusCode);
        }

        [Fact]
        public async Task UpdateBook_WhenNotExists_ReturnNotFound()
        {
            var bookIdToUpdate = 2;
            var bookDto = new BookDto { Genre = "Fiction", Id = 2, LoanedQuantity = 2, ISBN = "377-383-292-1", NumberOfCopies = 1, NumberOfPages = 120, Publisher = "publisher", ReleaseYear = 2010, Status = BookStatus.Available, Title = "Adeventure in London", Version = new byte[] { 1, 2, 3 } };
            var mock = new Mock<IBooksService>();
            mock.Setup(s => s.CheckExistsAsync(bookIdToUpdate)).ReturnsAsync(false);
            var controller = GetController(mock);

            //act
            var result = await controller.UpdateBookAsync(bookIdToUpdate, bookDto);
            Assert.IsAssignableFrom<NotFoundResult>(result);
            Assert.Equal(StatusCodes.Status404NotFound, (result as NotFoundResult).StatusCode);
        }

        [Fact]
        public async Task UpdateBook_WhenConcurrencyException_ReturnResultConflict()
        {
            var bookIdToUpdate = 2;
            var bookDto = new BookDto { Genre = "Fiction", Id = 2, LoanedQuantity = 2, ISBN = "377-383-292-1", NumberOfCopies = 1, NumberOfPages = 120, Publisher = "publisher", ReleaseYear = 2010, Status = BookStatus.Available, Title = "Ahead of Time", Version = new byte[] { 1, 2, 3 } };

            var mock = new Mock<IBooksService>();
            mock.Setup(s => s.CheckExistsAsync(bookIdToUpdate)).ReturnsAsync(true);
            mock.Setup(s => s.UpdateBookAsync(bookIdToUpdate, bookDto)).ThrowsAsync(new DbUpdateConcurrencyException());

            var controller = GetController(mock);

            //act
            var result = await controller.UpdateBookAsync(bookIdToUpdate, bookDto);
            Assert.IsAssignableFrom<ConflictObjectResult>(result);
            Assert.Equal(StatusCodes.Status409Conflict, (result as ConflictObjectResult).StatusCode);
        }

        public void Dispose()
        {
            // no-op
        }


        private IEnumerable<BookDto> GetBooksData()
        {
            return new List<BookDto>
            {
                new BookDto { 
                    Id = 1,
                    Genre="Fiction",
                    ISBN="273-373-282-2",
                    LoanedQuantity=1,
                    Publisher="Bentley",
                    NumberOfCopies=2,
                    NumberOfPages=120,
                    ReleaseYear=1990,
                    Status=Data.Models.BookStatus.Available,
                    Title="Winter in Strasbourg",
                    Version=new byte[]{0x00, 0x01 },
                    Authors = new int[] {1}                    
                },
                new BookDto
                {
                    Id=2,
                    Genre="History",
                    ISBN="283-228-283-28",
                    LoanedQuantity=2,
                    Publisher="Macmillan",
                    NumberOfCopies=5,
                    NumberOfPages=300,
                    ReleaseYear=2000,
                    Status= Data.Models.BookStatus.Available,
                    Title="Memory of a Geisha",
                    Version=new byte[]{0x00, 0x02 },
                    Authors = new int[] {2}
                },
                new BookDto
                {
                    Id=3,
                    Genre="History",
                    ISBN="393-283-293-1",
                    LoanedQuantity=1,
                    NumberOfCopies=5,
                    NumberOfPages=259,
                    Publisher="Hachette livre",
                    ReleaseYear=2001,
                    Status= Data.Models.BookStatus.Available,
                    Title="Gone with the wind",
                    Version=new byte[]{0x00,0x03},
                    Authors = new int[] {3}
                }
            };
        }
    }
}