
namespace ACST.AWS.Common
{
    using System;
    using System.Linq;

    /// <summary>
    /// <see cref="T:System.Exception"/> Extension Methods
    /// </summary>
    public static class ExceptionExtensions
    {
        /// <summary>
        /// Returns the innermost original exception from a nested exception 
        /// </summary>
        /// <param name="ex">Outer exception</param>
        /// <returns>Inner exception original exception</returns>
        public static Exception GetOriginalException(this Exception ex)
        {
            if (ex.InnerException == null) return ex;

            return ex.InnerException.GetOriginalException();
        }
    }
}
