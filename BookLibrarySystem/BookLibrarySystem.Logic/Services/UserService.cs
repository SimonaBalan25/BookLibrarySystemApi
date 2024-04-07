using AutoMapper;
using BookLibrarySystem.Data;
using BookLibrarySystem.Data.Models;
using BookLibrarySystem.Logic.DTOs;
using BookLibrarySystem.Logic.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BookLibrarySystem.Logic.Services
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _dbContext;
        private ILogger<UserService> _logger;
        private readonly IMapper _mapper;

        public UserService(ApplicationDbContext dbContext, ILogger<UserService> logger, IMapper mapper)
        {
            _dbContext = dbContext;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<IEnumerable<UserDto>> GetUsersAsync()
        {
            return await _dbContext.Users.Select(u => 
                        new UserDto() 
                        {
                            Id = u.Id,
                            Name = u.Name,
                            Address = u.Address,
                            BirthDate = u.BirthDate,
                            Email = u.Email,
                            EmailConfirmed = u.EmailConfirmed,
                            PhoneNumber = u.PhoneNumber,
                            PhoneNumberConfirmed = u.PhoneNumberConfirmed,
                            Status = u.Status,
                            UserName = u.UserName
                        }).ToListAsync();
        }

        public async Task<ApplicationUser> GetByUsernameAsync(string username)
        {
            return await _dbContext.Users.Where(u => u.Email.Equals(username)).FirstOrDefaultAsync();
        }

        public async Task<ApplicationUser> GetByIdAsync(string id)
        {
            return await _dbContext.Users.Where(u => u.Id.Equals(id)).SingleOrDefaultAsync();
        }

        public async Task<bool> CanBeDeletedAsync(string id)
        {
            return ((await _dbContext.Loans.Where(l => l.ApplicationUserId.Equals(id) && !l.Status.Equals(LoanStatus.Finalized)).ToListAsync()).Count() > 0) ||
                ((await _dbContext.Reservations.Where(r=>r.ApplicationUserId.Equals(id) && r.Status.Equals(ReservationStatus.Active)).ToListAsync()).Count() > 0);
        }

        public async Task<ApplicationUser?> AddUserAsync(UserDto newUser)
        {
            try
            {
                var dbUser = _mapper.Map<ApplicationUser>(newUser);
                await _dbContext.Users.AddAsync(dbUser);
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

        public async Task<bool> UpdateUserAsync(string id, UserDto updatedUser)
        {
            try
            {                
                var dbUser = await _dbContext.Users.FindAsync(id);
                dbUser.Address = updatedUser.Address;
                dbUser.PhoneNumber = updatedUser.PhoneNumber;
                dbUser.PhoneNumberConfirmed = updatedUser.PhoneNumberConfirmed;
                dbUser.Email = updatedUser.Email;   
                dbUser.Name = updatedUser.Name;
                dbUser.UserName = updatedUser.UserName;
                dbUser.EmailConfirmed = updatedUser.EmailConfirmed;
                dbUser.BirthDate = updatedUser.BirthDate;
                dbUser.Email = updatedUser.Email;
                //dbUser.Loans = updatedUser.Loans;
                //dbUser.Reservations = updatedUser.Reservations;
                dbUser.Status = updatedUser.Status;
                //dbUser.WaitingList = updatedUser.WaitingList;
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

        public async Task<bool> BlockUserAsync(string userId)
        {
            var selectedUser = await _dbContext.Users.FindAsync(userId);
            selectedUser.Status = UserStatus.Blocked;
            await _dbContext.SaveChangesAsync();    
            return true;
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
