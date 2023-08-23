

namespace LibraryWorkerService.Interfaces
{
    public interface IProcessLoansService
    {
        Task DoWorkAsync(CancellationToken stoppingToken);

        //Task<int> BlockUsersNotReturningBook();
    }
}
