

using BookLibrarySystem.Data.Models;

namespace BookLibrarySystem.Common.Models
{
    public class PagedResponse
    {
        public IOrderedQueryable<Book> Rows { get; set; }  

        public int TotalItems { get; set; }
    }
}
