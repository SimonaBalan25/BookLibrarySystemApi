

using BookLibrarySystem.Data.Models;

namespace BookLibrarySystem.Common.Models
{
    public class PagedResponse<T>
    {
        public IOrderedQueryable<T> Rows { get; set; }  

        public int TotalItems { get; set; }
    }
}
