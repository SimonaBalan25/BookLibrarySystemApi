using BookLibrarySystem.Logic.DTOs;
using BookLibrarySystem.Logic.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookLibrarySystem.Web.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("getUsers")]
        [Authorize(Roles ="Administrator")]
        public async Task<IActionResult> GetUsersAsync()
        {
            var allUsers = await _userService.GetUsersAsync();
            return Ok(allUsers);
        }

        [HttpPost("addUser")]
        public async Task<IActionResult> AddUserAsync(UserDto newUser)
        {
            var dbUser = await _userService.AddUserAsync(newUser);
            return CreatedAtAction("AddUser", new { id = newUser.Id }, dbUser);
        }

        [HttpPut("updateUser")]
        [Authorize(Roles ="Administrator")]
        public async Task<IActionResult> UpdateUserAsync(string userId, [FromBody]UserDto updatedUser)
        {
            if (string.IsNullOrEmpty(userId) || userId != updatedUser.Id)
            {
                return BadRequest("UserId is invalid !");
            }

            if (! await _userService.CheckUserExistsAsync(userId))
            {
                return NotFound("No user in the database matching this id !");
            }
            
            var result = await _userService.UpdateUserAsync(userId, updatedUser);

            return StatusCode(StatusCodes.Status200OK, "User was updated successfully in the database !");
        }

        [HttpDelete]
        [Authorize(Roles ="Administrator")]
        public async Task<IActionResult> DeleteUserAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest("Invalid user id");
            }

            var result = await _userService.DeleteUserAsync(id);

            return StatusCode(StatusCodes.Status200OK, "User was deleted successfully");
        }
    }
}
