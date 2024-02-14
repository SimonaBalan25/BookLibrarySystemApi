using AutoMapper;
using BookLibrarySystem.Data.Models;
using BookLibrarySystem.Logic.DTOs;
using BookLibrarySystem.Logic.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookLibrarySystem.Web.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class AuthorsController : ControllerBase
    {
        private readonly IAuthorsService _authorsService;
        private readonly IMapper _authorMapper;

        public AuthorsController(IAuthorsService authorsService, IMapper mapper)
        {
            _authorsService = authorsService;
            _authorMapper = mapper;
        }

        [HttpGet()]
        public async Task<IActionResult> GetAllAsync()
        {
            var authors = await _authorsService.GetAuthorsAsync();

            if (authors == null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Problem in getting the authors from the database");
            }

            return StatusCode(StatusCodes.Status200OK, authors);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAuthorAsync(int id)
        {
            var author = await _authorsService.GetAuthorAsync(id);

            if (author == null)
            {
                return NotFound();
            }

            return Ok(author);
        }

        [HttpPost]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> AddAuthorAsync(AuthorDto newAuthor)
        {
            var author = await _authorsService.AddAuthorAsync(newAuthor);    

            if (author == null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }

            return CreatedAtAction("AddAuthor", new { id = newAuthor.Id }, newAuthor);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> UpdateAuthorAsync(int id, [FromBody] AuthorDto modifiedAuthor)
        {
            if (id != modifiedAuthor.Id)
            {
                return BadRequest("not the same author");
            }

            if (!await _authorsService.CheckExistsAsync(id))
            {
                return NotFound("Author was not found");
            }

            var updated = await _authorsService.UpdateAuthorAsync(modifiedAuthor);    

            if (!updated)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error in updating the author in the database");
            }

            return Ok();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> DeleteAuthorAsync(int id)
        {
            if (!await _authorsService.CheckExistsAsync(id))
            {
                return NotFound();
            }

            var deleted = await _authorsService.DeleteAuthorAsync(id);

            if (!deleted)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }

            return Ok();
        }
    }
}
