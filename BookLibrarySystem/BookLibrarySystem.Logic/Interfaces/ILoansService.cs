
using BookLibrarySystem.Data.Models;

namespace BookLibrarySystem.Logic.Interfaces
{
    public interface ILoansService
    {
        Task<IEnumerable<BookLoan>> GetAllActiveAsync();
    }
}
