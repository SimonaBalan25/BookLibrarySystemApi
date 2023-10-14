
namespace LibraryWorkerService.Interfaces
{
    public interface ISetBkdEmail
    {
        Task SendRenewalPeriodEmail(string emailTo, string userName, string book, DateTime renewTime);

        Task SendBlockUserEmail(string emailTo, string userName, string book);

        Task SendReservationExpiredEmail(string emailTo, string userName, string book);
    }
}
