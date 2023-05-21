﻿using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookLibrarySystem.Data.Models
{
    public class Book
    {
        [Key]
        [Required]
        //[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string ISBN { get; set; }

        public string Title { get; set; }

        public string Publisher { get; set; }

        public int ReleaseYear { get; set; }

        public string Genre { get; set; }

        public int NumberOfCopies { get; set; }

        public int LoanedQuantity { get; set; }

        public int NumberOfPages { get; set; }

        [JsonIgnore]
        public virtual ICollection<Author> Authors { get; set; }

        [JsonIgnore]
        public virtual ICollection<BookAuthor> BookAuthors { get; set; }

        public virtual ICollection<BookLoan> Loans { get; set; }

        public virtual ICollection<Reservation> Reservations { get; set; }

        public virtual ICollection<WaitingList> WaitingList { get; set; }   
    }
}
