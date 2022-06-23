
namespace ACST.BizTalk.Integration.Common
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;

    /// <summary>
    /// Repository query Count extentions 
    /// </summary>
    public static class EntityCountExtensions
    {
        /// <summary>
        /// Count specified entities in repository
        /// </summary>
        public static int Count<T>(this IRepositoryBase<T> repository) where T : EntityBase
        {
            return repository.All().Count();
        }

        /// <summary>
        /// Count specified entities in repository
        /// </summary>
        public static int Count<T>(this IRepositoryBase<T> repository, Expression<Func<T, bool>> predicate) where T : EntityBase
        {
            return repository.All().Count(predicate);
        }
    }
}

