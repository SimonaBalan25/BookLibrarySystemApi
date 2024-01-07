using BookLibrarySystem.Data.Models;
using BookLibrarySystem.Logic.DTOs;
using BookLibrarySystem.Logic.Interfaces;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookLibrarySystem.Web.Controllers
{
    [Route("[controller]")]
    [Authorize]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly IBooksService _booksService;
        private readonly TelemetryClient _logger;
        

        public BooksController(IBooksService booksService, TelemetryClient logger)
        {
            _booksService = booksService;
            _logger = logger;
            
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            _logger.TrackTrace("Start:BooksController-GetAllAsync", Microsoft.ApplicationInsights.DataContracts.SeverityLevel.Information, new Dictionary<string, string> { { "source", "BooksController" } });
            var books = await _booksService.GetBooksAsync();
            _logger.TrackTrace("End BooksController-GetAllAsync");
            return StatusCode(StatusCodes.Status200OK, books);
        }

        [HttpGet("getBySearchCriteria")]
        public async Task<IActionResult> GetBySearchCriteria(string sortDirection, int pageIndex=1, int pageSize=10, string sortColumn="",  [FromQuery]Dictionary<string,string> filters=null)
        {
            var pagedResponseBooks = await _booksService.GetBySearchFilters(sortDirection, pageIndex, pageSize, sortColumn,  filters);

            return Ok(new { Books = pagedResponseBooks.Rows.ToList(), TotalItems = pagedResponseBooks.TotalItems });
        }

        [HttpGet("GetAllForListing")]
        public async Task<IActionResult> GetAllForListingAsync()
        {
            _logger.TrackTrace("Start:BooksController-GetAllAsync", Microsoft.ApplicationInsights.DataContracts.SeverityLevel.Information, new Dictionary<string, string> { { "source", "BooksController" } });
            var books = await _booksService.GetBooksForListingAsync();
            _logger.TrackTrace("End BooksController-GetAllAsync");
            return StatusCode(StatusCodes.Status200OK, books);
        }

        [HttpGet("search")]
        public async Task<IEnumerable<Book>> SearchBooksAsync(string keyword)
        {
            return await _booksService.SearchBooksAsync(keyword);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetBook(int id)
        {
            _logger.TrackTrace("Start: BooksController - GetBookById");
            var book = await _booksService.GetBookAsync(id);
            _logger.TrackTrace("End: BooksController - GetBookById");
            return StatusCode(StatusCodes.Status200OK, book);
        }

        [HttpPost]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> AddBook(BookDto bookDto)
        {
            var dbBook = await _booksService.AddBookAsync(bookDto, bookDto.Authors);

            return CreatedAtAction("AddBook", new { id = dbBook.Id }, dbBook);
        }

        [HttpPost("borrow")]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> BorrowBookAsync([FromQuery]int bookId, [FromQuery]string appUserId)
        {
            var exists = await _booksService.CheckExistsAsync(bookId);
            if (!exists)
                return NotFound("Book does not exist");

            var response = await _booksService.CanBorrowAsync(bookId, appUserId);
            if (!response.Allowed)
                return BadRequest(response.Reason);

            var result = await _booksService.BorrowBookAsync(bookId, appUserId);
            if (result > 0)
            {
                return Ok(result);
            }
            return StatusCode(StatusCodes.Status500InternalServerError, "There was a problem in borrowing the selected book !");
        }


        [HttpPut("return")]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> ReturnBookAsync([FromQuery]int bookId, [FromQuery]string appUserId)
        {
            var exists = await _booksService.CheckExistsAsync(bookId);

            if (!exists)
                return NotFound("Book does not exist");

            var result = await _booksService.ReturnBookAsync(bookId, appUserId);
            if (result > 0)
            {
                return Ok(result);
            }

            return StatusCode(StatusCodes.Status500InternalServerError, "There was a problem in returning that book.");
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> UpdateBookAsync(int id, [FromBody] BookDto updatedBook)
        {
            if (id == 0 || id != updatedBook.Id)
                return StatusCode(StatusCodes.Status400BadRequest, "Invalid id");

            var exists = await _booksService.CheckExistsAsync(id);

            if (!exists)
                return StatusCode(StatusCodes.Status404NotFound, "The book doesn't exist in the library !");

            var result = await _booksService.UpdateBookAsync(id, updatedBook);

            if (result)
            {
                return Ok("The book was updated successfully !");
            }

            return StatusCode(StatusCodes.Status500InternalServerError, "An error happened in the update book process !");
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> DeleteBookAsync(int id)
        {
            if (id == 0)
                return StatusCode(StatusCodes.Status400BadRequest, "Invalid id");
            var exists = await _booksService.CheckExistsAsync(id);

            if (!exists)
            {
                return StatusCode(StatusCodes.Status404NotFound, "The book selected to be deleted was not found in the library !");
            }

            //var selectedBook = await _libraryService.GetBookAsync(id);
            var result = await _booksService.DeleteBookAsync(id);

            if (result)
                return StatusCode(StatusCodes.Status200OK, "Book was deleted successfully");

            return StatusCode(StatusCodes.Status500InternalServerError, "The selected book could not be deleted from the library ");
        }

        [HttpPost("reserve")]
        [Authorize(Roles ="NormalUser")]
        public async Task<IActionResult> ReserveBookAsync([FromQuery]int bookId, [FromQuery]string appUserId)
        {
            if (bookId <= 0)
                return BadRequest("BookId should be a positive number");

            if (!await _booksService.CheckExistsAsync(bookId))
                return NotFound($"Book with id {bookId} was not found in the database");

            var response = await _booksService.CanReserveAsync(bookId, appUserId);
            if (!response.Allowed)
                return BadRequest(response.Reason);
            return Ok(await _booksService.ReserveBookAsync(bookId, appUserId));
        }

        [HttpPut("cancelReservation")]
        [Authorize(Roles ="NormalUser")]
        public async Task<IActionResult> CancelReservationAsync([FromQuery] int bookId, [FromQuery] string appUserId)
        {
            if (bookId < 0)
                return BadRequest("BookId should be a positive number");

            if (!await _booksService.CheckExistsAsync(bookId))
                return NotFound($"Book with id {bookId} was not found in the database");

            return Ok(await _booksService.CancelReservationAsync(bookId, appUserId));
        }
    }
}
