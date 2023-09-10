using BookLibrarySystem.Logic.Interfaces;
using LibraryWorkerService.Interfaces;

namespace LibraryWorkerService.Services
{
    public class ProcessReservations : IProcessReservations
    {
        private readonly IReservationsService _reservationsService;
        private readonly IBooksService _booksService;
        private readonly ISendEmail _emailService;
        private readonly IUserService _userService;

        public ProcessReservations(IReservationsService reservationsService, IBooksService booksService,
            ISendEmail emailService, IUserService userService) 
        {
            _reservationsService = reservationsService;
            _booksService = booksService;
            _userService = userService;
            _emailService = emailService;
        }

        public async Task DoWorkAsync(CancellationToken stoppingToken)
        {
            var allActiveReservations = await _reservationsService.GetAllActiveAsync();
            foreach (var reservation in allActiveReservations)
            {
                if (DateTime.Now > reservation.ReservedDate && DateTime.Now.Subtract(reservation.ReservedDate).Days > 5)
                {
                    var book = await _booksService.GetBookAsync(reservation.BookId);
                    var user = await _userService.GetByIdAsync(reservation.ApplicationUserId);
                    await _emailService.SendReservationExpiredEmail(user?.Email, user?.Name, book.Title);
                    await _reservationsService.SetExpiredAsync(reservation.Id);
                }
            }
        }
    }
}
