
namespace ACST.BizTalk.Integration.Common
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;

    /// <summary>
    /// Repository query Single extentions 
    /// </summary>
    public static class EntitySingleExtensions
    {
        /// <summary>
        /// Select Single entities from repository
        /// </summary>
        public static T Single<T>(this IRepositoryBase<T> repository) where T : EntityBase
        {
            return repository.All().Single();
        }

        /// <summary>
        /// Select Single entities from repository
        /// </summary>
        public static T Single<T>(this IRepositoryBase<T> repository, Expression<Func<T, bool>> predicate) where T : EntityBase
        {
            return repository.All().Single(predicate);
        }

        /// <summary>
        /// Select Single entities from repository
        /// </summary>
        public static T Single<T>(this IRepositoryBase<T> repository, ISpecification<T> spec) where T : EntityBase
        {
            return repository.All().SingleOrDefault(spec.Predicate);
        }

        /// <summary>
        /// Select Single or Default entities from repository
        /// </summary>
        public static T SingleOrDefault<T>(this IRepositoryBase<T> repository) where T : EntityBase
        {
            return repository.All().SingleOrDefault();
        }

        /// <summary>
        /// Select Single or Default entities from repository
        /// </summary>
        public static T SingleOrDefault<T>(this IRepositoryBase<T> repository, Expression<Func<T, bool>> predicate) where T : EntityBase
        {
            return repository.All().SingleOrDefault(predicate);
        }

        /// <summary>
        /// Select Single or Default entities from repository
        /// </summary>
        public static T SingleOrDefault<T>(this IRepositoryBase<T> repository, ISpecification<T> spec) where T : EntityBase
        {
            return repository.All().SingleOrDefault(spec.Predicate);
        }
    }
}

