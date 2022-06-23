
namespace ACST.AWS.Common
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;
    
    /// <summary>
    /// Specification pattern base type.
    /// </summary>
    [System.Diagnostics.DebuggerStepThrough()]
    public class Specification<E> 
        : ISpecification<E> where E : class
    {
        private readonly Expression<Func<E, bool>> _predicate;

        public Expression<Func<E, bool>> Predicate
        {
            get { return _predicate; }
        }

        #region Constructor

        /// <summary>
        /// Initialize a new instance of <see cref="Specification"/>
        /// </summary>
        /// <param name="predicate"></param>
        public Specification(Expression<Func<E, bool>> predicate)
        {
            _predicate = predicate;
        }
        #endregion

        #region Methods

        /// <summary>
        /// Determine if the candidate entity satisfies the specification.
        /// </summary>
        /// <param name="candidate"></param>
        /// <returns>Return true if candidate satisifies specification, false if not.</returns>
        public bool IsSatisfiedBy(E candidate)
        {
            return IsSatisfiedBy(candidate, false);
        }

        /// <summary>
        /// Determine if the candidate entity satisfies the specification.
        /// </summary>
        /// <param name="candidate">Entity to test</param>
        /// <param name="allowNullCandidate">Allow null entite as candidate without throwing an exception. Null entities always fail selection.</param>
        /// <returns>Return true if candidate satisifies specification, false if not.</returns>
        public bool IsSatisfiedBy(E candidate, bool allowNullCandidate)
        {
            if (allowNullCandidate && candidate == null)
                return false;

            return _predicate.Compile().Invoke(candidate);
        }

        /// <summary>
        /// Determine if the candidate entity fails to satisfy the specification.
        /// </summary>
        /// <param name="candidate">Entity to test</param>
        /// <returns>Return true if candidate fails to satisify specification, false otherwise.</returns>
        public bool IsNotSatisfiedBy(E candidate)
        {
            return !IsSatisfiedBy(candidate);
        }

        /// <summary>
        /// Determine if the candidate entity fails to satisfy the specification.
        /// </summary>
        /// <param name="candidate">Entity to test</param>
        /// <param name="allowNullCandidate">Allow null entites as candidates. Null entities always fail selection</param>
        /// <returns>Return true if candidate fails to satisify specification, false otherwise.</returns>
        public bool IsNotSatisfiedBy(E candidate, bool allowNullCandidate)
        {
            return !IsSatisfiedBy(candidate, allowNullCandidate);
        }

        public Specification<E> And(ISpecification<E> other)
        {
            //var otherInvoke = Expression.Invoke(other.Predicate, this._predicate.Parameters);
            //var newExpression = Expression.MakeBinary(ExpressionType.AndAlso, _predicate.Body, otherInvoke);
            //return new CompositeSpecification<E>(Expression.Lambda<Func<E, bool>>(newExpression, _predicate.Parameters));

            return new Specification<E>(this._predicate.And(other.Predicate));
        }

        public Specification<E> Or(ISpecification<E> other)
        {
            // ToDo: Cleanup -- Breaks ObjectQuery.ToTraceString()
            //var otherInvoke = Expression.Invoke(other.Predicate, this._predicate.Parameters);
            //var newExpression = Expression.MakeBinary(ExpressionType.OrElse, _predicate.Body, otherInvoke);
            //return new CompositeSpecification<E>(Expression.Lambda<Func<E, bool>>(newExpression, _predicate.Parameters));

            return new Specification<E>(this._predicate.Or(other.Predicate));
        }

        public Specification<E> Not()
        {
            var negativeExpression = Expression.Lambda<Func<E, bool>>(Expression.Not(_predicate.Body), _predicate.Parameters);

            return new Specification<E>(negativeExpression);
        }
        #endregion
    }
}