//***************************************************************************************
//* FILE:  DateTimeExtensions.cs
//* 
//***************************************************************************************
//* MODIFICATION LOG:
//* 
//* WHO                      WHEN        WHAT
//* -----------------------  ----------  ------------------------------------------------
//* Brian E. Moore           12/05/2018  Created
//***************************************************************************************

namespace ACST.AWS.Common
{
    using System;

    /// <summary>
    /// <see cref="T:System.DateTime"/> Extension Methods
    /// </summary>
    //[System.Diagnostics.DebuggerStepThrough]
    public static class DateTimeExtensions
    {
        /// <summary>Null safe method which formats current <see cref="T:System.DateTime"/> object.</summary>
        /// <returns>The Null safe formated datetime value.</returns>
        /// <remarks>Null value returns null without throwing an exception.</remarks>
        public static string ToSafeString(this DateTime? dt, string format = "") 
        {
            return dt == null ? null : ((DateTime)dt).ToString(format);
        }
    }
}
