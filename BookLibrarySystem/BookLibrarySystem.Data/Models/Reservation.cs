using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookLibrarySystem.Data.Models
{
    public class Reservation
    {
        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [ForeignKey("ApplicationUser")]
        public string ApplicationUserId { get; set; }

        [ForeignKey("Book")]
        public int BookId { get; set; }

        public DateTime ReservedDate { get; set; }

        public ReservationStatus Status { get; set; }
    }

    public enum ReservationStatus
    {
        Pending = 0,
        Active=1,
        Finalized = 2,
        Cancelled = 3,
        Expired=4
    }
}
