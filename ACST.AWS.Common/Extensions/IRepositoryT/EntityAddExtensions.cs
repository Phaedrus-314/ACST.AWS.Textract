
namespace ACST.BizTalk.Integration.Common
{
    using System.Collections.Generic;
    
    /// <summary>
    /// Repository query Add extentions 
    /// </summary>
    public static class EntityAddExtensions
    {
        /// <summary>
        /// Add specified entities to repository
        /// </summary>
        public static void Add<T>(this IRepository<T> repository, IEnumerable<T> entities) where T : EntityBase
        {
            if (entities != null)
            {
                foreach (var entity in entities)
                {
                    repository.Add(entity);
                }
            }
        }
    }
}

