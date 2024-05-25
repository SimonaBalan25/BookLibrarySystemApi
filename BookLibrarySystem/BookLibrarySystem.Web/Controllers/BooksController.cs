using BookLibrarySystem.Data.Models;
using BookLibrarySystem.Logic.DTOs;
using BookLibrarySystem.Logic.Interfaces;
using BookLibrarySystem.Web.Filters;
using Microsoft.ApplicationInsights;
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
        private readonly TelemetryClient _logger;
        

        public BooksController(IBooksService booksService, TelemetryClient logger)
        {
            _booksService = booksService;
            _logger = logger;
            
        }

        [HttpGet]
        [ETag]
        public async Task<IActionResult> GetAllAsync()
        {
            _logger.TrackTrace("Start:BooksController-GetAllAsync", Microsoft.ApplicationInsights.DataContracts.SeverityLevel.Information, new Dictionary<string, string> { { "source", "BooksController" } });
            var books = await _booksService.GetBooksAsync();
            _logger.TrackTrace("End BooksController-GetAllAsync");
            return StatusCode(StatusCodes.Status200OK, books);
        }

        [HttpGet("getBySearchCriteria")]
        [ETag]
        public async Task<IActionResult> GetBySearchCriteria(string sortDirection, int pageIndex=1, int pageSize=10, string sortColumn="",  [FromQuery]Dictionary<string,string> filters=null)
        {
            var pagedResponseBooks = await _booksService.GetBySearchFilters(sortDirection, pageIndex, pageSize, sortColumn,  filters);

            return Ok(new { Books = pagedResponseBooks.Rows.ToList(), TotalItems = pagedResponseBooks.TotalItems });
        }

        [HttpGet("GetAllForListing")]
        [ETag]
        public async Task<IActionResult> GetAllForListingAsync()
        {
            _logger.TrackTrace("Start:BooksController-GetAllAsync", Microsoft.ApplicationInsights.DataContracts.SeverityLevel.Information, new Dictionary<string, string> { { "source", "BooksController" } });
            var books = await _booksService.GetBooksForListingAsync();
            _logger.TrackTrace("End BooksController-GetAllAsync");
            return StatusCode(StatusCodes.Status200OK, books);
        }

        [HttpGet("GetAllWithRelatedInfo/{userId}")]
        public async Task<IActionResult> GetAllWithRelatedInfo(string userId)
        {
            _logger.TrackTrace("Start:BooksController-GetAllWithRelatedInfo", SeverityLevel.Information, new Dictionary<string, string> { { "source", "BooksController" } });
            var booksWithRelatedInfo = await _booksService.GetBooksWithRelatedInfo(userId);
            _logger.TrackTrace("End BooksController-GetAllWithRelatedInfo");
            return StatusCode(StatusCodes.Status200OK, booksWithRelatedInfo);
        }

        [HttpGet("search")]
        public async Task<IEnumerable<Book>> SearchBooksAsync(string keyword)
        {
            return await _booksService.SearchBooksAsync(keyword);
        }

        [HttpGet("{id:int}")]
        [ETag]
        public async Task<IActionResult> GetBook(int id)
        {
            _logger.TrackTrace("Start: BooksController - GetBookById");
            var book = await _booksService.GetBookAsync(id);
            _logger.TrackTrace("End: BooksController - GetBookById");
            return StatusCode(StatusCodes.Status200OK, book);
        }

        [HttpPost]
        [Authorize(Roles = "Administrator")]
        [ETag]
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


        [HttpPut("return")]
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
                return StatusCode(StatusCodes.Status400BadRequest, "Invalid id");

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
                return StatusCode(StatusCodes.Status400BadRequest, "Invalid id");
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
                return StatusCode(StatusCodes.Status200OK);

            return StatusCode(StatusCodes.Status500InternalServerError);
        }

        [HttpPost("reserve")]
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

        [HttpPut("cancelReservation")]
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
