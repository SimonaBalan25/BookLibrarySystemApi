﻿using BookLibrarySystem.Data.Models;
using BookLibrarySystem.Logic.DTOs;
using BookLibrarySystem.Logic.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BookLibrarySystem.Web.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    [EnableCors("AllowAllHeadersPolicy")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly UserManager<ApplicationUser> _userManager;

        public UsersController(IUserService userService, UserManager<ApplicationUser> userManager)
        {
            _userService = userService;
            _userManager = userManager;
        }

        [HttpGet("getUsersAsync")]
        [Authorize(Roles ="Administrator")]
        public async Task<IActionResult> GetUsersAsync()
        {
            var allUsers = await _userService.GetUsersAsync();
            return Ok(allUsers);
        }

        [HttpGet("getUsersWithInfoAsync")]
        [Authorize(Roles ="Administrator")]
        public async Task<IActionResult> GetUsersWithInfoAsync()
        {
            var usersWithInfo = await _userService.GetUsersWithInfoAsync();

            return Ok(new { Users = usersWithInfo, TotalItems = usersWithInfo.Count() });
        }

        [HttpPost("addUserAsync")]
        public async Task<IActionResult> AddUserAsync(UserDto newUser)
        {
            var dbUser = await _userService.AddUserAsync(newUser);
            return CreatedAtAction("AddUser", new { id = newUser.Id }, dbUser);
        }

        [HttpPut("{id}")]
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

            if (!await _userService.CanBeDeletedAsync(id))
            {
                return StatusCode(StatusCodes.Status400BadRequest, "User has loan or reservation active. Delete those first.");
            }

            var result = await _userService.DeleteUserAsync(id);

            return StatusCode(StatusCodes.Status200OK, "User was deleted successfully");
        }

        [HttpPatch]
        public async Task<IActionResult> DeactivateUserAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest("Invalid user id");
            }

            var result = await _userService.BlockUserAsync(id);
            return StatusCode(StatusCodes.Status200OK, "User is deactivated. He is not able anymore to do loans or reservations.");
        }

        [HttpGet("user-roles")]
        [Authorize(Roles ="Administrator,NormalUser")]
        public async Task<IActionResult> GetUserRolesAsync()
        {
            var nameIdValue = User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
            var user = await _userService.GetByIdAsync(nameIdValue);
            var roles = await _userManager.GetRolesAsync(user);
            return Ok(roles);
        }
    }
}
