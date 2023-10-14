using BookLibrarySystem.Data;
using BookLibrarySystem.Data.Models;
using BookLibrarySystem.Logic.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BookLibrarySystem.Logic.Services
{
    public class ReservationsService : IReservationsService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ILogger<ReservationsService> _logger;

        public ReservationsService(ApplicationDbContext dbContext, ILogger<ReservationsService> logger)
        {
            _dbContext = dbContext; 
            _logger = logger;
        }

        public async Task<IEnumerable<Reservation>> GetAllActiveAsync()
        {
            return await _dbContext.Reservations.Where(r=>r.Status.Equals(ReservationStatus.Active)).ToListAsync();
        }

        public async Task<bool> SetExpiredAsync(int id)
        {
            using (var dbTransaction = await _dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    var selectedReservation = await _dbContext.Reservations.FindAsync(id);
                    int result;
                    if (selectedReservation != null)
                    {
                        selectedReservation.Status = ReservationStatus.Expired;
                        result = await _dbContext.SaveChangesAsync();
                    }

                    //cancel also the records from the WaitingList
                    var waitingListForExpiredReservation = await _dbContext.WaitingList.Where(wl => wl.BookId.Equals(selectedReservation.BookId)).ToListAsync();
                    foreach (var waitingList in waitingListForExpiredReservation)
                    {
                        waitingList.Position = -1;
                    }
                    result = await _dbContext.SaveChangesAsync();
                    await dbTransaction.CommitAsync();
                    return result > 0;
                }
                catch (Exception ex)
                {
                    _logger.LogError("Something happened during setting reservation expired");
                    await dbTransaction.RollbackAsync();
                    throw;
                }
            }
            
        }
    }
}
