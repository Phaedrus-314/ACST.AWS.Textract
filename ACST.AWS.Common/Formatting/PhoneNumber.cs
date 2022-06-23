
namespace ACST.AWS.Common
{
    using System;
    using System.Linq;

    public class PhoneNumber 
        : FormattedStringBase
    {
        #region Constructors

        public PhoneNumber(string value)
            : base(value, true) { }

        public PhoneNumber(string value, bool throwExceptions)
            : base(value, throwExceptions) { }

        public PhoneNumber(string formattedPhone = null, string unformattedPhone = null)
            : base(formattedPhone, unformattedPhone, true) { }
        #endregion

        #region Overrides

        protected override void ValidateParameters(string formattedValue = null, string unformattedValue = null, bool throwExceptions = false)
        {
            base.ValidateParameters(formattedValue, unformattedValue, throwExceptions);

            if (formattedValue != null && formattedValue.Length < 13)
            {
                isParameterSetValid = false;
                if (throwExceptions)
                    throw new ArgumentOutOfRangeException("Formatted Phone Number, must contain 13 or more characters.");
            }


            if (unformattedValue != null && unformattedValue.Length < 10)
            {
                isParameterSetValid = false;
                if (throwExceptions)
                    throw new ArgumentOutOfRangeException("Unformatted Phone Number, must contain 10 or more integers.");
            }
        }

        protected override string ApplyFormatting(string value)
        {
            return string.Format("({0}) {1}-{2}", value.Substring(0, 3), value.Substring(3, 3), value.Substring(6, 4));
        }

        protected override string RemoveFormatting(string value)
        {
            return value.Replace("-", string.Empty)
                        .Replace("(", string.Empty)
                        .Replace(")", string.Empty)
                        .Replace(".", string.Empty)
                        .Replace(" ", string.Empty)
                        .Trim();
        }

        protected override bool isFormatted(string value)
        {
            bool hasDelimiter = false;

            hasDelimiter = value.Contains("(");
            hasDelimiter = hasDelimiter || value.Contains(")");
            hasDelimiter = hasDelimiter || value.Contains("-");
            hasDelimiter = hasDelimiter || value.Contains(".");
            hasDelimiter = hasDelimiter || value.Contains(" ");

            return hasDelimiter;
        }
        #endregion
    }
}
