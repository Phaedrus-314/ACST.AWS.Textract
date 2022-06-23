
namespace ACST.BizTalk.Integration.Common
{
    using System;
    using System.Diagnostics;
    using System.Linq;
    using System.Linq.Expressions;

    /// <summary>
    /// Repository query Find extentions 
    /// </summary>
    [DebuggerStepThrough]
    public static class EntityFindExtensions
    {

        static BooleanSwitch TraceLinqSwitch = new BooleanSwitch("TraceLinq",
                                                                 "Create Linq query verbose trace.");

        /// <summary>
        /// Find selected entities in repository
        /// </summary>
        public static IQueryable<T> Find<T>(this IRepositoryBase<T> repository, Expression<Func<T, bool>> predicate) where T : EntityBase
        {
            var query = repository.All().Where(predicate);
            if (TraceLinqSwitch.Enabled)
                Logger.TraceVerbose(((System.Data.Entity.Infrastructure.DbQuery<T>)query).ToString());
            //    Logger.TraceVerbose(((System.Data.Objects.ObjectQuery)query).ToTraceString());

            //return repository.All().Where(predicate);
            return query;
        }

        /// <summary>
        /// Find selected entities in repository
        /// </summary>
        public static IQueryable<T> Find<T>(this IRepositoryBase<T> repository, ISpecification<T> criteria) where T : EntityBase
        {
            //return repository.All().AsExpandable().Where(criteria.Predicate);
            //            string str = (((System.Data.Objects.ObjectQuery)result).ToTraceString());
            //to
            //string str = (((DbQuery<Product>)result).ToString());

            
            var query = repository.All().Where(criteria.Predicate);
            if (TraceLinqSwitch.Enabled)
                Logger.TraceVerbose(((System.Data.Entity.Infrastructure.DbQuery<T>)query).ToString());
                //Logger.TraceVerbose(((System.Data.Objects.ObjectQuery)query).ToTraceString());

            //return repository.All().Where(criteria.Predicate);
            return query;
        }

        //public static IQueryable<T> Find<T>(this IRepositoryReadOnly<T> repository, ISpecification<T> criteria) where T : EntityBase
        //{
        //    var query = repository.All().Where(criteria.Predicate);
        //    return repository.All().Where(criteria.Predicate);
        //}


        //public static IQueryable<T> Find<T>(this IRepository<T> repository, ISpecification<T> criteria) where T : System.Data.Objects.DataClasses.EntityObject
        //{
        //    //return repository.All().AsExpandable().Where(criteria.Predicate);
        //    var query = repository.All().Where(criteria.Predicate);
        //    //Console.WriteLine(((System.Data.Objects.ObjectQuery)query).ToTraceString());
        //    return repository.All().Where(criteria.Predicate);
        //}

        //public static IQueryable<T> Find<T>(this IRepository<T> repository, ISpecification<T> criteria) //where T : System.Data.Objects.DataClasses.EntityObject
        //{
        //    return repository.All().Where(criteria.IsSatisfied());
        //}
    }
}

