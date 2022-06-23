
namespace ACST.AWS.Common
{
    using System;
    using System.Linq.Expressions;
    
    /// <summary>
    /// Interface for Specification rules
    /// </summary>
    public interface ISpecification<E> where E : class
    {
        /// <summary>
        /// The criteria of the Specification.
        /// </summary>
        Expression<Func<E, bool>> Predicate { get; }

        /// <summary>
        /// Determine if the candidate entity satisfies the specification.
        /// </summary>
        bool IsSatisfiedBy(E candidate);

        /// <summary>
        /// Logical And of this and an other specification
        /// </summary>
        /// <returns>this and the other</returns>
        Specification<E> And(ISpecification<E> other);

        /// <summary>
        /// Logical Or of this and an other specification
        /// </summary>
        /// <returns>this or the other</returns>
        Specification<E> Or(ISpecification<E> other);

        /// <summary>
        /// Logical Not of this specificaion
        /// </summary>
        /// <returns>Not this</returns>
        Specification<E> Not();
    }

    //ToDo: decide about this ussage:

    //public class EligibleForExportRule : ISpecificationRule<tbl_BTS_Claims>
    //{
    //    public Expression<Func<tbl_BTS_Claims, bool>> IsSatisfied()
    //    {
    //        return c => c.ClaimStatus == null;
    //    }
    //}

    ///// <summary>
    ///// Inteface for SpecificationRule pattern.
    ///// </summary>
    //public interface ISpecificationRule<E> where E : class
    //{
    //    Expression<Func<E, bool>> IsSatisfied();
    //}
}