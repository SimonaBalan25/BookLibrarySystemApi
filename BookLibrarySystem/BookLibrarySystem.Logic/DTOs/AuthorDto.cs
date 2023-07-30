

namespace BookLibrarySystem.Logic.DTOs
{
    public class AuthorDto
    {
        public int Id { get; set; } 

        public string Name { get; set; }    

        public string Country { get; set; } 

        public IEnumerable<int> Books { get; set; } = new List<int>();    
    }
}
