using AutoMapper;
using BookLibrarySystem.Common.Models;
using BookLibrarySystem.Data;
using BookLibrarySystem.Data.Models;
using BookLibrarySystem.Logic.DTOs;
using BookLibrarySystem.Logic.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using System.Linq.Dynamic.Core;

namespace BookLibrarySystem.Logic.Services
{
    public class AuthorsService : IAuthorsService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ILogger<AuthorsService> _logger;
        private readonly IMapper _mapper;

        public AuthorsService(ApplicationDbContext dbContext, ILogger<AuthorsService> logger, IMapper mapper) 
        { 
            _dbContext = dbContext;
            _logger = logger;
            _mapper = mapper;
        }
        
        public async Task<bool> CheckExistsAsync(int id)
        {
            return await _dbContext.Authors.FindAsync(id) != null;
        }

        public async Task<IEnumerable<AuthorDto>> GetAuthorsAsync()
        {
            try
            {
                return await _dbContext.Authors.Select(a =>
                new AuthorDto()
                {
                    Id = a.Id,
                    Name = a.Name,
                    Country = a.Country,
                    Books = a.Books.Select(b => b.Id).ToList()
                }).ToListAsync();
            }
            catch
            {
                var sourceMessage = $"AuthorsService - GetAuthors method - there was an error";
                _logger.LogError(sourceMessage);
                throw;
            }
        }

        public async Task<PagedResponse<AuthorDto>> GetAuthorsBySortColumnAsync(string sortDirection, string sortColumn)
        {
            IQueryable<AuthorDto> filteredAuthors = (await GetAuthorsAsync()).AsQueryable();
            var totalCount = filteredAuthors.Count();

            IOrderedQueryable<AuthorDto> orderedAuthors = filteredAuthors as IOrderedQueryable<AuthorDto>;
            if (!string.IsNullOrEmpty(sortColumn))
            {
                // Build the dynamic sorting expression
                var orderByExpression = $"{sortColumn} {sortDirection}";

                // Apply dynamic sorting
                orderedAuthors = filteredAuthors.OrderBy(orderByExpression);

            }
            var list = orderedAuthors.ToList();
            return new PagedResponse<AuthorDto> { Rows = orderedAuthors, TotalItems = totalCount };
        }

        public async Task<AuthorDto?> GetAuthorAsync(int id)
        {
            try
            {
                return await _dbContext.Authors.AsNoTracking()
                    .Select(a => new AuthorDto { 
                        Id = a.Id,
                        Name = a.Name,
                        Country = a.Country,
                        Books = a.Books.Select(a=>a.Id).ToList()
                    })
                    .FirstAsync(ent => ent.Id.Equals(id));
            }
            catch
            {
                _logger.LogError("AuthorsService - GetAuthorById - The author could not be found !");
                throw;
            }
        }

        public async Task<Author?> AddAuthorAsync(AuthorDto author)
        {
            Author? dbAuthor = null;
            using (var dbTransaction = await _dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    dbAuthor = _mapper.Map<Author>(author);
                    await _dbContext.Authors.AddAsync(dbAuthor);
                    _dbContext.Entry(dbAuthor).State = EntityState.Added;
                    await _dbContext.SaveChangesAsync();

                    foreach (var bookId in author.Books)
                    {
                        await _dbContext.BookAuthors.AddAsync(new BookAuthor { AuthorId = dbAuthor.Id, BookId = bookId });
                    }
                    int added = await _dbContext.SaveChangesAsync();
                    await dbTransaction.CommitAsync();
                }
                catch
                {
                    _logger.LogError("AuthorsService - AddAuthor - Author could not be added");
                    throw;
                }
            }

            return await _dbContext.Authors.FindAsync(dbAuthor.Id);
        }

        public async Task<bool> UpdateAuthorAsync(AuthorDto author)
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
            using (IDbContextTransaction transaction = await _dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    var booksWrittenBy = await _dbContext.BookAuthors.Where(ba => ba.AuthorId == id).ToListAsync();
                    foreach (var authorBook in booksWrittenBy)
                    {
                        _dbContext.BookAuthors.Remove(authorBook);
                    }

                    var dbAuthor = await _dbContext.Authors.FindAsync(id);
                    _dbContext.Authors.Remove(dbAuthor);

                    var result = await _dbContext.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return result > 0;
                }
                catch
                {
                    _logger.LogError("AuthorsService - DeleteAuthor method - an error occurred");
                    await transaction.RollbackAsync();
                    throw;
                }
            }
        }

        public async Task<bool> AssignBooksAsync(int id, string[] booksIds)
        {
            using (var dbTransaction = await _dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    var author = _dbContext.Authors.Where(a => a.Id.Equals(id)).FirstOrDefault();
                    var existentAuthorBooks = _dbContext.BookAuthors.Where(ba => ba.AuthorId.Equals(id)).AsEnumerable();
                    _dbContext.BookAuthors.RemoveRange(existentAuthorBooks);
                    await _dbContext.SaveChangesAsync();

                    foreach (var bookId in booksIds)
                    {
                        _dbContext.BookAuthors.Add(new BookAuthor { AuthorId = id, BookId = Int32.Parse(bookId) });
                    }
                    await _dbContext.SaveChangesAsync();
                    await dbTransaction.CommitAsync();
                    return true;
                }
                catch (Exception ex)
                {
                    await dbTransaction.RollbackAsync();
                    return false;
                }
            }
        }
    }
}
