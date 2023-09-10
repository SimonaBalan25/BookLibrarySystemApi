
namespace LibraryWorkerService.Interfaces
{
    public interface IProcessReservations
    {
        Task DoWorkAsync(CancellationToken stoppingToken);
    }
}
