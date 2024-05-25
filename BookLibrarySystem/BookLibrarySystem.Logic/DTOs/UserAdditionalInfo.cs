

using BookLibrarySystem.Data.Data.Configuration;

namespace BookLibrarySystem.Logic.DTOs
{
    public class UserAdditionalInfo
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }   

        public string Roles { get; set; }

        public int LoansNumber { get; set; }

        public int ReservationsNumber { get; set; }
        public string Status { get; set; }
    }
}
