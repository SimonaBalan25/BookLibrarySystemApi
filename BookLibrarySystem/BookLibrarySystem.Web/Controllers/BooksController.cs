using BookLibrarySystem.Data.Models;
using BookLibrarySystem.Logic.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookLibrarySystem.Web.Controllers
{
    [Route("[controller]")]
    [Authorize]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly ILibraryService _libraryService;

        public BooksController(ILibraryService libraryService)
        {
            _libraryService = libraryService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            var books = await _libraryService.GetBooksAsync();
            if (books == null)
            {
                return StatusCode(StatusCodes.Status204NoContent, "No books in the database.");
            }

            return StatusCode(StatusCodes.Status200OK, books);
        }

        [HttpGet("search")]
        public async Task<IEnumerable<Book>> SearchBooksAsync(string keyword)
        {
            return await _libraryService.SearchBooksAsync(keyword);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetBook(int id)
        {
            var book = await _libraryService.GetBookAsync(id);
            
            if (book == null)
            {
                return StatusCode(StatusCodes.Status204NoContent, $"No books in the database with the id {id}");
            }
            return StatusCode(StatusCodes.Status200OK, book);
        }

        [HttpPost]
        public async Task<IActionResult> AddBook(Book book)
        {

            var dbBook = await _libraryService.AddBookAsync(book);

            if (dbBook == null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"{book.Title} could not be added.");
            }
            return CreatedAtAction("GetBook", new { id = book.Id }, book);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateBook(int id, Book book)
        {
            if (id != book.Id)
            {
                return BadRequest();
            }
            var dbBook = await _libraryService.UpdateBookAsync(id, book);

            if (dbBook == null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Book {book.Title} could not be updated.");
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBook(int id)
        {
            var book = await _libraryService.GetBookAsync(id);
            (bool status, string message) = await _libraryService.DeleteBookAsync(book);

            if (status == false)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, message);
            }

            return StatusCode(StatusCodes.Status200OK, book);
        }

    }
}
