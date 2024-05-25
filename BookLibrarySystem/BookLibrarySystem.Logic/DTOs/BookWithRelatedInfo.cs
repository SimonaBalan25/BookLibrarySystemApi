

namespace BookLibrarySystem.Logic.DTOs
{
    public class BookWithRelatedInfo : BookDto
    {
        public string User { get; set; }  

        public bool IsReservedByUser { get; set; }
    }
}
