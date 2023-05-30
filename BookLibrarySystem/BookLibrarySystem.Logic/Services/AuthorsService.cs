

using BookLibrarySystem.Data;
using BookLibrarySystem.Data.Models;
using BookLibrarySystem.Logic.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BookLibrarySystem.Logic.Services
{
    public class AuthorsService : IAuthorsService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ILogger<AuthorsService> _logger;

        public AuthorsService(ApplicationDbContext dbContext) 
        { 
            _dbContext = dbContext;
        }
        
        public async Task<bool> CheckExistsAsync(int id)
        {
            return await _dbContext.Authors.FindAsync(id) != null;
        }

        public async Task<IEnumerable<Author>> GetAuthorsAsync()
        {
            try
            {
                return await _dbContext.Authors.Select(a =>
                new Author()
                {
                    Id = a.Id,
                    Name = a.Name,
                    Country = a.Country,
                    Books = a.AuthorBooks.Select(b => b.Book).ToList()
                }).ToListAsync();
            }
            catch (Exception ex) 
            {
                _logger.LogError($"GetAuthors error: {ex.Message}", ex);
                return null;
            }
        }

        public async Task<Author?> GetAuthorAsync(int id)
        {
            try
            {
                return await _dbContext.Authors.FindAsync(id);
            }
            catch(Exception ex)
            {
                _logger.LogError("The author could not be found !", ex);
                return null;
            }
        }

        public async Task<Author?> AddAuthorAsync(Author author)
        {            
            await _dbContext.Authors.AddAsync(author);
            int added = await _dbContext.SaveChangesAsync();

            if (added <= 0)
            {
                _logger.LogError("Author could not be added");
                return null;
            }

            return await _dbContext.Authors.FindAsync(author.Id);
        }

        public async Task<bool> UpdateAuthorAsync(Author author)
        {
            var dbAuthor = await GetAuthorAsync(author.Id);
            if (dbAuthor == null)
            {
                _logger.LogError("Author was not found to be updated !");
                return false;
            }

            dbAuthor.Name=author.Name;
            dbAuthor.Country = author.Country;
            _dbContext.Entry(dbAuthor).State = EntityState.Modified;
            return await _dbContext.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteAuthorAsync(int id)
        {
            var dbAuthor = await GetAuthorAsync(id);
            if (dbAuthor == null)
            {
                _logger.LogError("Author was not found to be deleted !");
                return false;
            }

            _dbContext.Authors.Remove(dbAuthor);
            return await _dbContext.SaveChangesAsync() > 0;
        }
    }
}
