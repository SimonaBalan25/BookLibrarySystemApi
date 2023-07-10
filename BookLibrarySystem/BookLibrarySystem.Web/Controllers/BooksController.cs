using AutoMapper;
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
        private readonly IMapper _mapper;

        public BooksController(IBooksService booksService, TelemetryClient logger, IMapper mapper)
        {
            _booksService = booksService;
            _logger = logger;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            _logger.TrackTrace("Start:BooksController-GetAllAsync", Microsoft.ApplicationInsights.DataContracts.SeverityLevel.Information, new Dictionary<string, string> { { "source", "BooksController" } });
            var books = await _booksService.GetBooksAsync();
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
            var book = _mapper.Map<Book>(bookDto);

            var dbBook = await _booksService.AddBookAsync(book, bookDto.Authors);

            return CreatedAtAction("AddBook", new { id = dbBook.Id }, book);
        }

        [HttpPost("borrow")]
        [Authorize(Roles = "Administrator")]
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
        [Authorize(Roles = "Administrator")]
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
        [Authorize(Roles = "Administrator")]
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
    }
}
