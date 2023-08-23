using BookLibrarySystem.Logic.Interfaces;
using LibraryWorkerService.Interfaces;

namespace LibraryWorkerService.Services
{
    public class ProcessReservationsService : IProcessReservationsService
    {
        private readonly IReservationsService _reservationsService;

        public ProcessReservationsService(IReservationsService reservationsService) 
        {
            _reservationsService = reservationsService;
        }

        public async Task DoWorkAsync(CancellationToken stoppingToken)
        {
            var allActiveReservations = await _reservationsService.GetAllActiveAsync();
            foreach (var reservation in allActiveReservations)
            {
                if (DateTime.Now > reservation.ReservedDate && DateTime.Now.Subtract(reservation.ReservedDate).Days > 5)
                {
                    await _reservationsService.SetExpiredAsync(reservation.Id);
                }
            }
            //return await Task.FromResult(true);
        }
    }
}
