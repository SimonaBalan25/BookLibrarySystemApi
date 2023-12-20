using System.Linq.Expressions;

namespace BookLibrarySystem.Common.Models
{
    public class FilterBy<TResult,TSource> : IFilterBy
    {
        private readonly Func<TSource, Func<TResult, bool>> function;

        public FilterBy(Func<TSource, Func<TResult, bool>> function)
        {
            this.function = function;
        }

        public dynamic Function => this.function;
    }
}
