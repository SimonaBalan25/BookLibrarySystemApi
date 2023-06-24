namespace BookLibrarySystem.Web.Entities
{
    public class ErrorDetails
    {
        public string? Message { get; set; }

        public string? Source { get; set; }
        public string? ErrorId { get; set; }
        public int StatusCode { get; set; }

        public string? StackTrace { get; set; }
    }
}
