
//namespace ACST.BizTalk.Integration.Common
//{
//    using System;
//    using System.Linq;
//    using System.Linq.Expressions;

//    /// <summary>
//    /// Repository query Single extentions 
//    /// </summary>
//    public static class EntityOrderExtensions
//    {
//        /// <summary>
//        /// Order entities in ascending order
//        /// </summary>
//        public static T OrderBy<T>(this IRepository<T> repository)
//        {
//            return repository.All().OrderBy();
//        }

//        /// <summary>
//        /// Order entities in ascending order
//        /// </summary>
//        public static T OrderBy<T>(this IRepository<T> repository, Expression<Func<T, TKey>> keySelector)
//        {
//            return repository.All().OrderBy(keySelector);
//        }

//        /// <summary>
//        /// Order entities in descending order
//        /// </summary>
//        public static T OrderByDescending<T>(this IRepository<T> repository)
//        {
//            return repository.All().OrderByDescending();
//        }

//        /// <summary>
//        /// Order entities in descending order
//        /// </summary>
//        public static T OrderByDescending<T>(this IRepository<T> repository, Expression<Func<T, TKey>> keySelector)
//        {
//            return repository.All().OrderByDescending(keySelector);
//        }
//    }
//}
