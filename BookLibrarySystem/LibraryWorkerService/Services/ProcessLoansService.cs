using BookLibrarySystem.Data.Models;
using BookLibrarySystem.Logic.Interfaces;
using LibraryWorkerService.Interfaces;

namespace LibraryWorkerService.Services
{
    public class ProcessLoansService : IProcessLoansService
    {
        private int _executionCount;
        private readonly IBooksService _booksService;
        private readonly IUserService _userService;
        private readonly ILoansService _loansService;
        private readonly ILogger<ProcessLoansService> _logger;

        public ProcessLoansService(IBooksService booksService, IUserService userService, 
            ILoansService loansService, ILogger<ProcessLoansService> logger) 
        {
            _booksService = booksService;
            _userService = userService;
            _loansService = loansService;
            _logger = logger;
        }

        public async Task DoWorkAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                ++_executionCount;
                _logger.LogInformation(
                    "{ServiceName} working, execution count: {Count}",
                    nameof(ProcessLoansService),
                    _executionCount);

                var blockedUsers = new List<string>();
                var loans = (await _loansService.GetAllLoansAsync()).ToList();//.Where(l => l.Status.Equals(LoanStatus.Active))
                foreach (var loan in loans)
                {
                    if (loan.Status == LoanStatus.Active && DateTime.Now > loan.DueDate && await _booksService.CanBeRenewed(loan.BookId, loan.ApplicationUserId))
                    {
                        //try to renew the loan
                        var renewed = await _booksService.RenewBookAsync(loan.BookId, loan.ApplicationUserId);
                    }

                    if (loan.Status == LoanStatus.Active && DateTime.Now > loan.DueDate && ! await _booksService.CanBeRenewed(loan.BookId, loan.ApplicationUserId))    
                    {
                        //block the user
                        var blocked = await _userService.BlockUserAsync(loan.ApplicationUserId);
                        if (!blockedUsers.Contains(loan.ApplicationUserId))
                        {
                            blockedUsers.Add(loan.ApplicationUserId);
                        }
                    }
                    
                    if (loan.Status == LoanStatus.Renewed && DateTime.Now > loan.DueDate)
                    {
                        //block the user
                        var blocked = await _userService.BlockUserAsync(loan.ApplicationUserId);
                        if (!blockedUsers.Contains(loan.ApplicationUserId))
                        {
                            blockedUsers.Add(loan.ApplicationUserId);
                        }
                    }
                }

                await Task.Delay(10_000, stoppingToken);
            }
        }
    }
}
