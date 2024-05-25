

using BookLibrarySystem.Data.Models;
using System.ComponentModel.DataAnnotations;

namespace BookLibrarySystem.Logic.DTOs
{
    public class BookDto
    {
        public int Id { get; set; } 

        public string ISBN { get; set; }

        public string Title { get; set; }

        public string Publisher { get; set; }

        public int ReleaseYear { get; set; }

        public string Genre { get; set; }

        public BookStatus Status { get; set; }

        public int NumberOfCopies { get; set; }

        public int LoanedQuantity { get; set; }

        public int NumberOfPages { get; set; }
        
        [Timestamp]
        public byte[] Version { get; set; }

        public IEnumerable<int> Authors { get; set; } = new List<int>();
    }
}
