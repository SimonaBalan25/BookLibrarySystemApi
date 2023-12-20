using System.Linq.Expressions;

namespace BookLibrarySystem.Common
{
    public class OrderBy<TResult, TSource> : IOrderBy
    {
        private readonly Expression<Func<TResult, TSource>> expression;

        public OrderBy(Expression<Func<TResult, TSource>> expression)
        {
            this.expression = expression;
        }

        public dynamic Expression => this.expression;
    }
}
