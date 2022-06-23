
namespace ACST.BizTalk.Integration.Common
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;

    /// <summary>
    /// Repository query Between extentions 
    /// </summary>
    [System.Diagnostics.DebuggerStepThrough]
    public static class EntityBetweenExtensions
    {
        /// <summary>
        /// Between operator on selected entities in repository
        /// </summary>
        public static IQueryable<T> Between<T, K>(this IQueryable<T> source,
                                                    Expression<Func<T, K>> keySelector,
                                                    K low, K high) where K 
                                        : EntityBase, IComparable<K>
        {
            Expression key = Expression.Invoke(keySelector, keySelector.Parameters.ToArray());
            
            Expression lowerBound = Expression.GreaterThanOrEqual(key, Expression.Constant(low));
            Expression upperBound = Expression.LessThanOrEqual(key, Expression.Constant(high));
            
            Expression and = Expression.AndAlso(lowerBound, upperBound);
            
            Expression<Func<T, bool>> lambda = Expression.Lambda<Func<T, bool>>(and, keySelector.Parameters);
            
            return source.Where(lambda);
        }
    }
}
