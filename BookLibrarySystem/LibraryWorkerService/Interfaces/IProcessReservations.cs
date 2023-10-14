
namespace LibraryWorkerService.Interfaces
{
    public interface IProcessReservations
    {
        Task DoWorkAsync(int executionCount);
    }
}
