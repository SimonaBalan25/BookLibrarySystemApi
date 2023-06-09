﻿using BookLibrarySystem.Data;
using BookLibrarySystem.Data.Models;
using BookLibrarySystem.Logic.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;

namespace BookLibrarySystem.Logic.Services
{
    public class AuthorsService : IAuthorsService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ILogger<AuthorsService> _logger;

        public AuthorsService(ApplicationDbContext dbContext, ILogger<AuthorsService> logger) 
        { 
            _dbContext = dbContext;
            _logger = logger;
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
                    Books = a.BookAuthors.Select(b => b.Book).ToList()
                }).ToListAsync();
            }
            catch
            {
                var sourceMessage = $"AuthorsService - GetAuthors method - there was an error";
                _logger.LogError(sourceMessage);
                throw;
            }
        }

        public async Task<Author?> GetAuthorAsync(int id)
        {
            try
            {
                return await _dbContext.Authors.AsNoTracking().FirstAsync(ent => ent.Id.Equals(id));
            }
            catch
            {
                _logger.LogError("AuthorsService - GetAuthorById - The author could not be found !");
                throw;
            }
        }

        public async Task<Author?> AddAuthorAsync(Author author)
        {
            try
            {
                await _dbContext.Authors.AddAsync(author);
                int added = await _dbContext.SaveChangesAsync();
            }
            catch
            {
                _logger.LogError("AuthorsService - AddAuthor - Author could not be added");
                throw;
            }

            return await _dbContext.Authors.FindAsync(author.Id);
        }

        public async Task<bool> UpdateAuthorAsync(Author author)
        {
            using (IDbContextTransaction transaction = await _dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    var dbAuthor = await GetAuthorAsync(author.Id);
                    dbAuthor.Name = author.Name;
                    dbAuthor.Country = author.Country;
                    _dbContext.Entry(dbAuthor).State = EntityState.Modified;
                    return await _dbContext.SaveChangesAsync() > 0;
                }
                catch
                {
                    _logger.LogError("AuthorsService - UpdateAuthor method - an error occurred");
                    await transaction.RollbackAsync();
                    throw;
                }
            }
        }

        public async Task<bool> DeleteAuthorAsync(int id)
        {
            try
            {
                var dbAuthor = await GetAuthorAsync(id);
                _dbContext.Authors.Remove(dbAuthor);
                return await _dbContext.SaveChangesAsync() > 0;
            }
            catch
            {
                _logger.LogError("AuthorsService - DeleteAuthor method - an error occurred");
                throw;
            }
        }
    }
}
