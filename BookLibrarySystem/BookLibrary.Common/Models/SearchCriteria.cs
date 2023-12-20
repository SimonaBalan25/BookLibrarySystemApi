
using System.ComponentModel;

namespace BookLibrarySystem.Common.Models
{
    public class SearchCriteria
    {
        public int PageSize { get; set; }   

        public int PageIndex { get; set; }

        public string SortColumn { get; set; }

        public SortDirection SortDirection { get; set; }

        public Dictionary<string,string> Filters { get; set; }
    }

    public enum SortDirection 
    {
        Asc = 0,
        Desc = 1
    }

}      
