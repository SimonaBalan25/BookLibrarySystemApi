﻿
namespace BookLibrarySystem.Logic.Entities
{
    public class CanGetBookResponse
    {
        public bool Allowed { get; set; }

        public string Reason { get; set; } 
        
        public IEnumerable<int> Borrowed { get; set; }
    }
}