namespace ACST.AWS.Common
{
    using System;

    /// <summary>
    /// <see cref="T:System.String"/> Extension Methods
    /// </summary>
    [System.Diagnostics.DebuggerStepThrough]
    public static class StringExtensions
    {
        /// <summary>
        /// Returns true if specified <see cref="T:System.String"/> is not empty or null.
        /// </summary>
        public static bool IsNotNullOrEmpty(this string value)
        {
            return !string.IsNullOrEmpty(value);
        }

        /// <summary>
        /// Returns true if specified <see cref="T:System.String"/> is empty or null.
        /// </summary>
        public static bool IsNullOrEmpty(this string value)
        {
            return string.IsNullOrEmpty(value);
        }

        public static bool IsNullOrWhiteSpace(this String str)
        {
            return string.IsNullOrWhiteSpace(str);
        }

        public static bool IsNotNullOrWhiteSpace(this String str)
        {
            return !string.IsNullOrWhiteSpace(str);
        }

        /// <summary>
        /// Replace the first occurance of specified search <see cref="T:System.String"/> with specified replace <see cref="T:System.String"/>.
        /// </summary>
        /// <param name="search">Search string to replace with new value</param>
        /// <param name="replace">Replacement string</param>
        /// <returns></returns>
        public static string ReplaceFirst(this string value, string search, string replace)
        {
            int i = value.IndexOf(search);

            if (i < 0) return value;

            return value.Substring(0, i) + replace + value.Substring(i + search.Length);
        }

        public static string SafeSubstring(this String str, int startIndex, int length = int.MaxValue)
        {
            if (str.IsNullOrEmpty())
                return null;

            if (str.Length - startIndex <= length)
                return str.Substring(startIndex);

            return str.Substring(startIndex, length);
        }

        /// <summary>Null safe method which removes all leading and trailing white-space characters from the current
        /// <see cref="T:System.String"/> object.</summary>
        /// <returns>The string that remains after all white-space characters are removed
        /// from the start and end of the current string.</returns>
        /// <remarks>Null value returns null without throwing an exception.</remarks>
        public static string SafeTrim(this string value)
        {
            return value == null ? null : value.Trim();
        }

        /// <summary>
        /// Format specified <see cref="T:System.String"/> for normalized HTML display.
        /// </summary>
        public static string ToHtmlString(this string value)
        {
            //All blank spaces would be replaced for html subsitute of blank space(&nbsp;) 
            value = value.Replace(" ", "&nbsp;");
            value = value.Replace("\t", "&nbsp;&nbsp;&nbsp;&nbsp;");

            //Carriage return & newline replaced to <br/>
            value = value.Replace("\r\n", "<br/>");
            value = value.Replace("\r", "<br/>");

            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.AppendFormat("<html><head><title></title></head><body><table border=0 width=95% cellpadding=0 cellspacing=0><tr><td>{0}</td></tr></table></body></html>", value);

            return sb.ToString();
        }

        /// <summary>
        /// Format specified <see cref="T:System.String"/> to Proper Case.
        /// Capatalize the first character of each word, with remaining lowercase.
        /// </summary>
        public static string ToProperCase(this string value)
        {
            if (value.IsNullOrEmpty())
            {
                return string.Empty;
            }

            char[] a = value.ToLower().ToCharArray();

            a[0] = char.ToUpper(a[0]);
            
            return new string(a);
        }
        /// <summary>
        /// Truncate current <see cref="T:System.String"/> object to the specified length.
        /// </summary>
        /// <remarks>Null value returns null without throwing an exception.</remarks>
        public static string Truncate(this string value, Int32 length, bool addEllipsis = false)
        {
            if (value == null) return null;

            if (length > value.Length) return value.Trim();

            return value.Substring(0, length).Trim() + (addEllipsis ? "..." : "");
        }

        public static string ToDateFormat(this string value)
        {
            DateTime dt;
            bool f = DateTime.TryParse(value, out dt);

            return f ? dt.ToString("yyyy-MM-dd") : null;
        }

        public static decimal ToDecimal(this string value)
        {
            if (string.IsNullOrEmpty(value))
                return 0;

            decimal d;
            if (!decimal.TryParse(value, out d))
                return 0;

            return d;
        }

        public static string CleanupForCurrency(this string value)
        {
            if (value.IsNullOrWhiteSpace())
                return null;

            value = value.ToUpper();
            value = value.Replace('O', '0');

            value = value.Replace(" 00", ".00");
            value = value.Replace(':', '.');
            value = value.Replace(';', '.');
            value = value.Replace(',', '.');
            value = value.Replace('|', '.');
            decimal d = 0;
            if (decimal.TryParse(value, out d))
                return Math.Abs(d).ToString();
            else
                return value; 
        }

        public static string Filter(this string value, Func<char, bool> filter)
        {
            return FormattedStringBase.Filter(value, filter);
        }

    }
}
