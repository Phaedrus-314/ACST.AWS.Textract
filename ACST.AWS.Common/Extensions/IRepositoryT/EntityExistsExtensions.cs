
namespace ACST.BizTalk.Integration.Common
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;

    /// <summary>
    /// Repository query Existance extentions 
    /// </summary>
    public static class EntityExistsExtensions
    {
        /// <summary>
        /// Test entity existance in repository
        /// </summary>
        public static bool Exists<T>(this IRepositoryBase<T> repository, Expression<Func<T, bool>> predicate) where T : EntityBase
        {
            return repository.All().SingleOrDefault(predicate) != null;
        }
    }
}

