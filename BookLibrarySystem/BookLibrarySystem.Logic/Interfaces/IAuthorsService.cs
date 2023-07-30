﻿using BookLibrarySystem.Data.Models;
using BookLibrarySystem.Logic.DTOs;

namespace BookLibrarySystem.Logic.Interfaces
{
    public interface IAuthorsService
    {
        Task<bool> CheckExistsAsync(int id);

        Task<IEnumerable<Author>> GetAuthorsAsync();

        Task<AuthorDto?> GetAuthorAsync(int id);

        Task<Author> AddAuthorAsync(Author author);

        Task<bool> UpdateAuthorAsync(AuthorDto author);

        Task<bool> DeleteAuthorAsync(int id);
    }
}
