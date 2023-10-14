using BookLibrarySystem.Logic.Interfaces;
using LibraryWorkerService.Interfaces;

namespace LibraryWorkerService.Services
{
    public class ProcessReservations : IProcessReservations
    {
        private readonly IReservationsService _reservationsService;
        private readonly IBooksService _booksService;
        private readonly ISetBkdEmail _emailService;
        private readonly IUserService _userService;
        private readonly ILogger<ProcessReservations> _logger;
        private const int MaximumNumberOfDaysTillExpire = 2;

        public ProcessReservations(IReservationsService reservationsService, IBooksService booksService,
            ISetBkdEmail emailService, IUserService userService, ILogger<ProcessReservations> logger) 
        {
            _reservationsService = reservationsService;
            _booksService = booksService;
            _userService = userService;
            _emailService = emailService;
            _logger = logger;
        }

        public async Task DoWorkAsync(int executionCount)
        {
            _logger.LogInformation(
                    "{ServiceName} working, execution count: {Count}",
                    nameof(ProcessReservations),
                    executionCount);

            var allActiveReservations = await _reservationsService.GetAllActiveAsync();
            foreach (var reservation in allActiveReservations)
            {
                if (DateTime.Now > reservation.ReservedDate && DateTime.Now.Subtract(reservation.ReservedDate).Days > MaximumNumberOfDaysTillExpire)
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
