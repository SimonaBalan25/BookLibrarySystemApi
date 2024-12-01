using BookLibrarySystem.Data.Models;
using BookLibrarySystem.Logic.DTOs;
using BookLibrarySystem.Logic.Interfaces;
using BookLibrarySystem.Web.Filters;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookLibrarySystem.Web.Controllers
{
    [Route("[controller]")]
    [Authorize]
    [ApiController]    
    public class BooksController : ControllerBase
    {
        private readonly IBooksService _booksService;
        private readonly ITelemetryService _loggerService;

        public BooksController(IBooksService booksService, ITelemetryService loggerService)
        {
            _booksService = booksService;
            _loggerService = loggerService;
        }

        [HttpGet]
        [ETag]
        public async Task<IActionResult> GetAllAsync()
        {
            _loggerService.TrackEvent("Start:BooksController-GetAllAsync", SeverityLevel.Information, new Dictionary<string, string> { { "source", "BooksController" } });
            var books = await _booksService.GetBooksAsync();
            
            _loggerService.TrackEvent("End BooksController-GetAllAsync");
            return StatusCode(StatusCodes.Status200OK, books);
        }

        [HttpGet("getBySearchCriteriaAsync")]
        [ETag]
        public async Task<IActionResult> GetBySearchCriteriaAsync(string sortDirection, int pageIndex=1, int pageSize=10, string sortColumn="", Dictionary<string,string> filters=null)
        {
            var pagedResponseBooks = await _booksService.GetBySearchFiltersAsync(sortDirection, pageIndex, pageSize, sortColumn,  filters);

            return Ok(new { Books = pagedResponseBooks.Rows.ToList(), TotalItems = pagedResponseBooks.TotalItems });
        }

        [HttpGet("GetAllForListingAsync")]
        [ETag]
        public async Task<IActionResult> GetAllForListingAsync()
        {
            _loggerService.TrackEvent("Start:BooksController-GetAllAsync", Microsoft.ApplicationInsights.DataContracts.SeverityLevel.Information, new Dictionary<string, string> { { "source", "BooksController" } });
            var books = await _booksService.GetBooksForListingAsync();
            _loggerService.TrackEvent("End BooksController-GetAllAsync");
            return StatusCode(StatusCodes.Status200OK, books);
        }

        [HttpGet("GetAllWithRelatedInfoAsync/{userId}")]
        public async Task<IActionResult> GetAllWithRelatedInfoAsync(string userId)
        {
            _loggerService.TrackEvent("Start:BooksController-GetAllWithRelatedInfoAsync", SeverityLevel.Information, new Dictionary<string, string> { { "source", "BooksController" } });
            var booksWithRelatedInfo = await _booksService.GetBooksWithRelatedInfoAsync(userId);
            _loggerService.TrackEvent("End BooksController-GetAllWithRelatedInfoAsync");
            return StatusCode(StatusCodes.Status200OK, booksWithRelatedInfo);
        }

        [HttpGet("searchAsync")]
        public async Task<IEnumerable<Book>> SearchBooksAsync(string keyword)
        {
            return await _booksService.SearchBooksAsync(keyword);
        }

        [HttpGet("{id:int}")]
        [ETag]
        public async Task<IActionResult> GetBookAsync(int id)
        {
            _loggerService.TrackEvent("Start: BooksController - GetBookById");
            var book = await _booksService.GetBookAsync(id);
            _loggerService.TrackEvent("End: BooksController - GetBookById");
            if (book == null) 
            {
                return NotFound();
            }

            return Ok(book);
        }

        [HttpPost]
        [Authorize(Roles = "Administrator")]
        [ETag]
        public async Task<IActionResult> AddBookAsync(BookDto bookDto)
        {
            var addedDbBook = await _booksService.AddBookAsync(bookDto, bookDto.Authors);

            return CreatedAtAction("AddBook", new { id = addedDbBook?.Id }, bookDto);
        }

        [HttpPost("borrowAsync")]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> BorrowBookAsync([FromQuery]int bookId, [FromQuery]string appUserId)
        {
            var exists = await _booksService.CheckExistsAsync(bookId);
            if (!exists)
                return NotFound();

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


        [HttpPut("returnAsync")]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> ReturnBookAsync([FromQuery]int bookId, [FromQuery]string appUserId)
        {
            var exists = await _booksService.CheckExistsAsync(bookId);

            if (!exists)
                return NotFound();
            
            var canReturnResponse = await _booksService.CanReturnAsync(bookId, appUserId);
            if (!canReturnResponse.Allowed)
            {
                return BadRequest(canReturnResponse.Reason);
            }

            var result = await _booksService.ReturnBookAsync(bookId, appUserId);
            if (result > 0)
            {
                return Ok(result);
            }

            return StatusCode(StatusCodes.Status500InternalServerError, "There was a problem in returning that book.");
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Administrator")]
        [ETag]
        public async Task<IActionResult> UpdateBookAsync(int id, [FromBody] BookDto updatedBook)
        {
            if (id == 0 || id != updatedBook.Id)
                return BadRequest("Invalid id");

            var exists = await _booksService.CheckExistsAsync(id);

            if (!exists)
                return NotFound();

            bool result;
            try
            {
                result = await _booksService.UpdateBookAsync(id, updatedBook);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return Conflict(ex.Message);
            }

            if (result)
            {
                return Ok();
            }

            return StatusCode(StatusCodes.Status500InternalServerError);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Administrator")]
        [ETag]
        public async Task<IActionResult> DeleteBookAsync(int id)
        {
            if (id == 0)
                return BadRequest("Invalid id");
            var exists = await _booksService.CheckExistsAsync(id);

            if (!exists)
            {
                return NotFound();
            }

            var response = await _booksService.CanDeleteAsync(id);
            if (!response.Allowed)
            {
                return BadRequest(response.Reason);
            }
            
            var result = await _booksService.DeleteBookAsync(id);

            if (result)
                return Ok();

            return StatusCode(StatusCodes.Status500InternalServerError);
        }

        [HttpPost("reserveAsync")]
        [Authorize(Roles ="NormalUser")]
        public async Task<IActionResult> ReserveBookAsync([FromQuery]int bookId, [FromQuery]string appUserId)
        {
            if (bookId <= 0)
                return BadRequest("BookId should be a positive number");

            if (!await _booksService.CheckExistsAsync(bookId))
                return NotFound();

            var response = await _booksService.CanReserveAsync(bookId, appUserId);
            if (!response.Allowed)
                return BadRequest(response.Reason);
            return Ok(await _booksService.ReserveBookAsync(bookId, appUserId));
        }

        [HttpPut("cancelReservationAsync")]
        [Authorize(Roles ="NormalUser")]
        public async Task<IActionResult> CancelReservationAsync([FromQuery] int bookId, [FromQuery] string appUserId)
        {
            if (bookId < 0)
                return BadRequest("BookId should be a positive number");

            if (!await _booksService.CheckExistsAsync(bookId))
                return NotFound();

            return Ok(await _booksService.CancelReservationAsync(bookId, appUserId));
        }        
    }
}
