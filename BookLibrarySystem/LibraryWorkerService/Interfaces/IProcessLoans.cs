

namespace LibraryWorkerService.Interfaces
{
    public interface IProcessLoans
    {
        Task DoWorkAsync(CancellationToken stoppingToken);
    }
}
