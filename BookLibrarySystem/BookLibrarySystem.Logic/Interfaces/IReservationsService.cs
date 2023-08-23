
using BookLibrarySystem.Data.Models;

namespace BookLibrarySystem.Logic.Interfaces
{
    public interface IReservationsService
    {
        Task<IEnumerable<Reservation>> GetAllActiveAsync();

        Task<bool> SetExpiredAsync(int id);
    }
}
