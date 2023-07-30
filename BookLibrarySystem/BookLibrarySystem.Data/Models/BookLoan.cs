using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookLibrarySystem.Data.Models
{
    public class BookLoan
    {
        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [ForeignKey("Book")]
        public int BookId { get; set; }

        [ForeignKey("ApplicationUser")]
        public string ApplicationUserId { get; set; }

        public DateTime BorrowedDate { get; set; }

        public DateTime? ReturnedDate { get; set; }

        public DateTime DueDate { get; set; }

        public bool Renewed { get; set; }

        public LoanStatus Status { get; set; }
    }

    public enum LoanStatus
    {
        Active = 0,
        Returned = 1,
        Overdue = 2,
        Renewed = 3,
        Reserved
    }
}
