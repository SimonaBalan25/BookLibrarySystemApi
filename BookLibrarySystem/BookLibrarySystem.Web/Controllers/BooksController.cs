using BookLibrarySystem.Data.Models;
using BookLibrarySystem.Logic.Interfaces;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
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
        [Authorize(Roles ="Administrator,NormalUser")]
        public async Task<IActionResult> GetAllAsync()
        {
            _logger.TrackTrace("Inside BooksController", SeverityLevel.Information, new Dictionary<string, string>() { });
            var books = await _booksService.GetBooksAsync();
            
            _logger.TrackTrace("Finish BooksController");
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
            var book = await _booksService.GetBookAsync(id);
            return StatusCode(StatusCodes.Status200OK, book);
        }

        [HttpPost]
        public async Task<IActionResult> AddBook(Book book)
        {

            var dbBook = await _booksService.AddBookAsync(book);

            return CreatedAtAction("AddBook", new { id = book.Id }, book);
        }

        [HttpPost("borrow")]
        public async Task<IActionResult> BorrowBookAsync(int bookId, string appUserId)
        {
            var exists = await _booksService.CheckExistsAsync(bookId);
            if (!exists)
                return NotFound("Book does not exist");

            var selectedBook = await _booksService.GetBookAsync(bookId);
            if (!_booksService.ValidateBorrowAsync(selectedBook))
                return BadRequest("Book cannot be borrowed, since all the copies are already borrowed");

            var result = await _booksService.BorrowBookAsync(selectedBook, appUserId);
            if (result > 0)
            {
                return Ok(result);
            }
            return StatusCode(StatusCodes.Status500InternalServerError, "There was a problem in borrowing the selected book !");
        }

        [HttpPost("return")]
        public async Task<IActionResult> ReturnBookAsync(int bookId, string appUserId)
        {
            var exists = await _booksService.CheckExistsAsync(bookId);

            if (!exists)
                return NotFound("Book does not exist");

            var selectedBook = await _booksService.GetBookAsync(bookId);
            var result = await _booksService.ReturnBookAsync(selectedBook, appUserId);
            if (result > 0)
            {
                return Ok(result);
            }

            return StatusCode(StatusCodes.Status500InternalServerError, "There was a problem in returning that book.");
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBookAsync(int id, [FromBody] Book updatedBook)
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
    }
}
