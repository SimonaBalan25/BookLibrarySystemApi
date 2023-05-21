using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookLibrarySystem.Data.Models
{
    public class Author
    {
        [Key]
        [Required]
        //[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string Name { get; set; }

        public string Country { get; set; }

        [JsonIgnore]
        public virtual ICollection<Book> Books { get; set;}

        [JsonIgnore]
        public ICollection<BookAuthor> AuthorBooks { get; set; }    
    }
}
