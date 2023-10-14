

namespace LibraryWorkerService.Interfaces
{
    public interface IProcessLoans
    {
        Task DoWorkAsync(int executionCount);
    }
}
