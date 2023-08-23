using Microsoft.AspNetCore.Identity;

namespace BookLibrarySystem.Data.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? Name { get;set; }

        public DateTime BirthDate { get;set; }

        public UserStatus Status { get; set; }

        public string? Address { get; set; }

        public virtual ICollection<BookLoan> Loans { get; set; }

        public virtual ICollection<Reservation> Reservations { get; set; }

        public virtual ICollection<WaitingList> WaitingList { get; set; }
    }

    public enum UserStatus
    {
        Active = 0,
        Blocked
    }
}