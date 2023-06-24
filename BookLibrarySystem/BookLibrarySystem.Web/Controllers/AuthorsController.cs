﻿using BookLibrarySystem.Data.Models;
using BookLibrarySystem.Logic.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BookLibrarySystem.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AuthorsController : ControllerBase
    {
        private readonly IAuthorsService _authorsService;

        public AuthorsController(IAuthorsService authorsService)
        {
            _authorsService = authorsService;
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
                return NotFound("Author was not found in the database !");
            }

            return Ok(author);
        }

        [HttpPost]
        public async Task<IActionResult> AddAuthorAsync([FromBody] Author newAuthor)
        {
            var author = await _authorsService.AddAuthorAsync(newAuthor);    

            if (author == null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error in adding new author in the database !");
            }

            return CreatedAtAction("AddAuthor", new { id = newAuthor.Id }, newAuthor);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAuthorAsync(int id, [FromBody] Author author)
        {
            if (id != author.Id)
            {
                return BadRequest("not the same author");
            }

            if (!await _authorsService.CheckExistsAsync(id))
            {
                return NotFound("Author was not found");
            }

            var updated = await _authorsService.UpdateAuthorAsync(author);    

            if (!updated)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error in updating the author in the database");
            }

            return Ok("Author was updated");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAuthorAsync(int id)
        {
            if (!await _authorsService.CheckExistsAsync(id))
            {
                return NotFound("Author was not found");
            }

            var deleted = await _authorsService.DeleteAuthorAsync(id);

            if (!deleted)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error in deleting the author from database !");
            }

            return Ok("Author was deleted");
        }
    }
}