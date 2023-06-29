using BookLibrarySystem.Data;
using BookLibrarySystem.Data.Models;
using BookLibrarySystem.Logic.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BookLibrarySystem.Logic.Services
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _dbContext;
        private ILogger<UserService> _logger;

        public UserService(ApplicationDbContext dbContext, ILogger<UserService> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<IEnumerable<ApplicationUser>> GetUsersAsync()
        {
            return await _dbContext.Users.ToListAsync();
        }

        public async Task<ApplicationUser> GetByUsernameAsync(string username)
        {
            return await _dbContext.Users.Where(u => u.Email.Equals(username)).FirstOrDefaultAsync();
        }

        public async Task<ApplicationUser> GetByIdAsync(string id)
        {
            return await _dbContext.Users.Where(u => u.Id.Equals(id)).FirstOrDefaultAsync();
        }

        public async Task<ApplicationUser?> AddUserAsync(ApplicationUser newUser)
        {
            try
            {
                await _dbContext.Users.AddAsync(newUser);
                _dbContext.Entry(newUser).State = EntityState.Added;
                await _dbContext.SaveChangesAsync();

                return await _dbContext.Users.FindAsync(newUser.Id);
            }
            catch
            {
                _logger.LogError("USersService - AddUser - an error occurred");
                throw;
            }
        }

        public async Task<bool> UpdateUserAsync(string id, ApplicationUser updatedUser)
        {
            try
            {
                var dbUser = await _dbContext.Users.FindAsync(id);
                dbUser.Address = updatedUser.Address;
                dbUser.PhoneNumber = updatedUser.PhoneNumber;
                dbUser.PhoneNumberConfirmed = updatedUser.PhoneNumberConfirmed;
                dbUser.EmailConfirmed = updatedUser.EmailConfirmed;
                dbUser.BirthDate = updatedUser.BirthDate;
                dbUser.Email = updatedUser.Email;
                dbUser.Loans = updatedUser.Loans;
                dbUser.Reservations = updatedUser.Reservations;
                dbUser.Status = updatedUser.Status;
                dbUser.WaitingList = updatedUser.WaitingList;
                _dbContext.Entry(updatedUser).State = EntityState.Modified;
                var result = await _dbContext.SaveChangesAsync();
                return result > 0;
            }
            catch
            {
                _logger.LogError("UserService - UpdateUser method - an error occurred");
                throw;
            }
        }

        public async Task<bool> DeleteUserAsync(string id)
        {
            try
            {
                var dbUser = await _dbContext.Users.FindAsync(id);
                _dbContext.Entry(dbUser).State = EntityState.Deleted;
                _dbContext.Users.Remove(dbUser);
                var result = await _dbContext.SaveChangesAsync();
                return result > 0;
            }
            catch
            {
                _logger.LogError("UserService - DeleteUser method - an error occurred");
                throw;
            }
        }

        public async Task<bool> CheckUserExistsAsync(string userId)
        {
            return await _dbContext.Users.FindAsync(userId) != null;
        }
    }
}
