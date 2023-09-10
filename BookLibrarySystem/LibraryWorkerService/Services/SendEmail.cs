using System.Net.Mail;
using System.Net;
using LibraryWorkerService.Interfaces;
using LibraryWorkerService.Models;

namespace LibraryWorkerService.Services
{
    public class SendEmail : ISendEmail
    {
        private readonly EmailSettings _emailSettings;
        private const string RenewalSubject = "Renew loan";
        private const string RenewalBody = "Hello {0}, your period of loan for book {1} was renewed until {2}.";
        private const string BlockSubject = "Block user";
        private const string BlockBody = "Hello {0}, your user was blocked because of not returning book {1}";
        private const string ReservationExpiredSubject = "Reservation expired";
        private const string ReservationExpiredBody = "Hello {0}, you reserved book {1}, but the reservation expired";

        public SendEmail(IConfiguration configuration)
        {
            _emailSettings = configuration.GetSection("EmailSettings").Get<EmailSettings>(); 
        }

        public async Task SendRenewalPeriodEmail(string emailTo, string userName, string book, DateTime renewDate)
        {
            var emailBody = string.Format(RenewalBody, userName, book, renewDate);
            await SendMailMessage(emailTo, RenewalSubject, emailBody);
        }

        public async Task SendBlockUserEmail(string emailTo, string userName, string book)
        {
            var emailBody = string.Format(BlockBody, userName, book);
            await SendMailMessage(emailTo, BlockSubject, emailBody);
        }

        public async Task SendReservationExpiredEmail(string emailTo, string userName, string book)
        {
            var emailBody = string.Format(ReservationExpiredBody, userName, book);
            await SendMailMessage(emailTo, ReservationExpiredSubject, emailBody);
        }

        private async Task SendMailMessage(string emailTo, string subject, string body)
        {
            using (MailMessage mail = new MailMessage())
            {
                mail.From = new MailAddress(_emailSettings.EmailFrom);
                mail.To.Add(emailTo);
                mail.Subject = subject;
                mail.Body = body;
                mail.IsBodyHtml = true;

                //mail.Attachments.Add(new Attachment("D:\\TestFile.txt"));//--Uncomment this to send any attachment  
                using (SmtpClient smtp = new SmtpClient(_emailSettings.SmtpAddress, _emailSettings.PortNumber))
                {
                    smtp.Credentials = new NetworkCredential(_emailSettings.EmailFrom, _emailSettings.Password);
                    smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                    smtp.EnableSsl = _emailSettings.EnableSsl;
                    await smtp.SendMailAsync(mail);
                }
            }
        }
    }
}
