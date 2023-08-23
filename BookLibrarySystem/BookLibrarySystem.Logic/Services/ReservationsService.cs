using BookLibrarySystem.Data;
using BookLibrarySystem.Data.Models;
using BookLibrarySystem.Logic.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BookLibrarySystem.Logic.Services
{
    public class ReservationsService : IReservationsService
    {
        private readonly ApplicationDbContext _dbContext;

        public ReservationsService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext; 
        }

        public async Task<IEnumerable<Reservation>> GetAllActiveAsync()
        {
            return await _dbContext.Reservations.Where(r=>r.Status.Equals(ReservationStatus.Active)).ToListAsync();
        }

        public async Task<bool> SetExpiredAsync(int id)
        {
            var selectedReservation = await _dbContext.Reservations.FindAsync(id);
            selectedReservation.Status = ReservationStatus.Expired;
            var result = await _dbContext.SaveChangesAsync();
            return result > 0;
        }
    }
}
