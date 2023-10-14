using BookLibrarySystem.Data.Models;
using BookLibrarySystem.Logic.Interfaces;
using LibraryWorkerService.Interfaces;

namespace LibraryWorkerService.Services
{
    public class ProcessLoans : IProcessLoans
    {        
        private readonly IBooksService _booksService;
        private readonly IUserService _userService;
        private readonly ILoansService _loansService;
        private readonly ISetBkdEmail _emailService;
        private readonly ILogger<ProcessLoans> _logger;

        public ProcessLoans(IBooksService booksService, IUserService userService, 
            ILoansService loansService, ISetBkdEmail emailService, ILogger<ProcessLoans> logger) 
        {
            _booksService = booksService;
            _userService = userService;
            _loansService = loansService;
            _emailService = emailService;
            _logger = logger;
        }

        public async Task DoWorkAsync(int executionCount)
        {

            _logger.LogInformation(
                "{ServiceName} working, execution count: {Count}",
                nameof(ProcessLoans),
                executionCount);

            var blockedUsers = new List<string>();
            var loans = (await _loansService.GetAllActiveAsync()).ToList();
            foreach (var loan in loans)
            {
                var user = await _userService.GetByIdAsync(loan.ApplicationUserId);

                if (loan.Status == LoanStatus.Active && 
                    DateTime.Now > loan.DueDate && 
                    await _booksService.CanRenewAsync(loan.BookId, loan.ApplicationUserId) &&
                    !user.Status.Equals(UserStatus.Blocked))
                {
                    //try to renew the loan
                    var dateRenewed = await _booksService.RenewBookAsync(loan.BookId, loan.ApplicationUserId);
                    var book = await _booksService.GetBookAsync(loan.BookId);
                    await _emailService.SendRenewalPeriodEmail(user?.Email, user?.UserName, book.Title, dateRenewed);
                    continue;
                }

                    
                if (loan.Status == LoanStatus.Active && 
                    DateTime.Now > loan.DueDate && 
                    ! await _booksService.CanRenewAsync(loan.BookId, loan.ApplicationUserId) &&
                    !user.Status.Equals(UserStatus.Blocked))    
                {
                    //block the user
                    var blocked = await _userService.BlockUserAsync(loan.ApplicationUserId);
                    var book = await _booksService.GetBookAsync(loan.BookId);
                        
                    await _emailService.SendBlockUserEmail(user?.Email, user?.UserName, book.Title);
                    if (!blockedUsers.Contains(loan.ApplicationUserId))
                    {
                        blockedUsers.Add(loan.ApplicationUserId);
                    }
                    continue;
                }
                    
                if (loan.Status == LoanStatus.Renewed && 
                    DateTime.Now > loan.DueDate &&
                    !user.Status.Equals(UserStatus.Blocked))
                {
                    //block the user
                    var blocked = await _userService.BlockUserAsync(loan.ApplicationUserId);
                    var book = await _booksService.GetBookAsync(loan.BookId);

                    await _emailService.SendBlockUserEmail(user?.Email, user?.UserName, book.Title);
                    if (!blockedUsers.Contains(loan.ApplicationUserId))
                    {
                        blockedUsers.Add(loan.ApplicationUserId);
                    }
                }
            }
        }
    }
}
