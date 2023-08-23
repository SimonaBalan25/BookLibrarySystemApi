﻿using BookLibrarySystem.Data;
using BookLibrarySystem.Data.Models;
using BookLibrarySystem.Logic.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BookLibrarySystem.Logic.Services
{
    public class LoansService : ILoansService
    {
        private readonly ApplicationDbContext _dbContext;

        public LoansService(ApplicationDbContext dbContext) 
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<BookLoan>> GetAllLoansAsync()
        {
            return await _dbContext.Loans.ToListAsync();
        }
    }
}
