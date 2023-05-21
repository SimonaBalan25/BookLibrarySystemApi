using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookLibrarySystem.Data.Models
{
    public class Reservation
    {
        [Key]
        [Required]
        //[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
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
        Finalized = 0,
        Cancelled,
        Pending
    }
}
