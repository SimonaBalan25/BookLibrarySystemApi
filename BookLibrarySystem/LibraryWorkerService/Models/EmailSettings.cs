
namespace LibraryWorkerService.Models
{
    public class EmailSettings
    {
        public string SmtpAddress { get; set; } 

        public int PortNumber { get;set; }

        public bool EnableSsl { get;set; }

        public string EmailFrom { get;set; }

        public string Password { get;set; }
    }
}
