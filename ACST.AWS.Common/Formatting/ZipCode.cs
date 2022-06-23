
namespace ACST.AWS.Common
{
    using System;
    using System.Linq;

    public class ZipCode
        : FormattedStringBase
    {
        #region Constructors

        public ZipCode(string value)
            : base(value, true) { }

        public ZipCode(string value, bool throwExceptions)
            : base(value, throwExceptions) { }

        public ZipCode(string formattedZipCode = null, string unformattedZipCode = null)
            : base(formattedZipCode, unformattedZipCode, true) { }
        #endregion

        #region Overrides

        protected override void ValidateParameters(string formattedValue = null, string unformattedValue = null, bool throwExceptions = false)
        {
            base.ValidateParameters(formattedValue, unformattedValue, throwExceptions);

            int[] validFormattedLengths = new int[] { 5, 10 };
            int[] validUnFormattedLengths = new int[] { 5, 9 };

            if (formattedValue != null && !validFormattedLengths.Contains(formattedValue.Length))
            {
                isParameterSetValid = false;
                if (throwExceptions)
                    throw new ArgumentOutOfRangeException("Formatted ZipCode must contain 5 or 10 integers.");
            }


            if (unformattedValue != null && !validUnFormattedLengths.Contains(unformattedValue.Length))
            {
                isParameterSetValid = false;
                if (throwExceptions)
                    throw new ArgumentOutOfRangeException("Unformatted ZipCode must contain 5 or 9 integers.");
            }
        }

        protected override string ApplyFormatting(string value)
        {
            if (value.Length == 9)
                return string.Format("{0}-{1}", value.Substring(0, 5), value.Substring(5, 4));
            else if (value.Length > 4)
                return string.Format("{0}", value.Substring(0, 5));
            else
                return null;
        }

        protected override string RemoveFormatting(string value)
        {
            if (value == null)
                return null;

            return value.Replace("-", string.Empty)
                        .Replace(" ", string.Empty)
                        .Trim();
        }

        protected override bool isFormatted(string value)
        {
            bool hasDelimiter = false;

            hasDelimiter = value.Contains("-");
            //hasDelimiter = hasDelimiter || value.Contains(")");
            //hasDelimiter = hasDelimiter || value.Contains("-");
            //hasDelimiter = hasDelimiter || value.Contains(".");
            //hasDelimiter = hasDelimiter || value.Contains(" ");

            return hasDelimiter;
        }
        #endregion

        public static string Format(string value)
        {
            if (value.IsNullOrEmpty())
                return null;

            ZipCode zipCode = new ZipCode(value.Truncate(10), false);

            return zipCode.WithFormatting;
        }
    }
}
