using LibraryWorkerService.Interfaces;
using BookLibrarySystem.Logic.Interfaces;
using BookLibrary.Common;

namespace LibraryWorkerService.Services
{
    public class SetBkdEmail : ISetBkdEmail
    {
        private readonly EmailSettings _emailSettings;
        private readonly ISendEmail _sendEmail;
        private const string RenewalSubject = "Renew loan";
        private const string RenewalBody = "Hello {0}, your period of loan for book {1} was renewed until {2}.";
        private const string BlockSubject = "Block user";
        private const string BlockBody = "Hello {0}, your user was blocked because of not returning book {1}";
        private const string ReservationExpiredSubject = "Reservation expired";
        private const string ReservationExpiredBody = "Hello {0}, you reserved book {1}, but the reservation expired";

        public SetBkdEmail(IConfiguration configuration, ISendEmail sendEmail)
        {
            _emailSettings = configuration.GetSection("EmailSettings").Get<EmailSettings>(); 
            _sendEmail = sendEmail;
        }

        public async Task SendRenewalPeriodEmail(string emailTo, string userName, string book, DateTime renewDate)
        {
            var emailBody = string.Format(RenewalBody, userName, book, renewDate);
            await _sendEmail.SendMailMessage(emailTo, RenewalSubject, emailBody);
        }

        public async Task SendBlockUserEmail(string emailTo, string userName, string book)
        {
            var emailBody = string.Format(BlockBody, userName, book);
            await _sendEmail.SendMailMessage(emailTo, BlockSubject, emailBody);
        }

        public async Task SendReservationExpiredEmail(string emailTo, string userName, string book)
        {
            var emailBody = string.Format(ReservationExpiredBody, userName, book);
            await _sendEmail.SendMailMessage(emailTo, ReservationExpiredSubject, emailBody);
        }
    }
}
