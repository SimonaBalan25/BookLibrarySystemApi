using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookLibrarySystem.Data.Models
{
    public class BookAuthor
    {
        [Key]
        [Required]
        //[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; } 

        [ForeignKey("Author")]
        public int AuthorId { get; set; }

        [ForeignKey("Book")]
        public int BookId { get; set; }

        [JsonIgnore]
        public Book Book { get; set; } = null!;

        [JsonIgnore]
        public Author Author { get; set; } = null!;
    }
}
