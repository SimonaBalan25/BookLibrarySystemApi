

namespace BookLibrarySystem.Common.Extensions
{
    public static class FilterByExtensions
    {
        public static IOrderedQueryable<T> Where<T>(this IOrderedQueryable<T> source, IFilterBy orderBy)
        {
            return Queryable.Where(source, orderBy.Function);
        }
    }
}
