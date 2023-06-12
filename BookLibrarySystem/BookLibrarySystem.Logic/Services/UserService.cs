using BookLibrarySystem.Data;
using BookLibrarySystem.Data.Models;
using BookLibrarySystem.Logic.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookLibrarySystem.Logic.Services
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _dbContext;

        public UserService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<ApplicationUser> GetByUsernameAsync(string username)
        {
            return await _dbContext.Users.Where(u => u.Email.Equals(username)).FirstOrDefaultAsync();
        }

        public async Task<ApplicationUser> GetByIdAsync(string id)
        {
            return await _dbContext.Users.Where(u => u.Id.Equals(id)).FirstOrDefaultAsync();
        }
    }
}
