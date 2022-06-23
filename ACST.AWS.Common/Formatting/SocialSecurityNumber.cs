
namespace ACST.AWS.Common
{
    using System;

    /// <summary>
    /// Social Security Number immutable formatted string
    /// </summary>
    [System.Diagnostics.DebuggerStepThrough()]
    public sealed class SocialSecurityNumber 
        : FormattedStringBase
    {
        public string WithSecurityEncoding 
        {
            get
            {
                Int32 number;
                if (Int32.TryParse(this.WithoutFormatting, out number))
                    return (number ^ 666666666).ToString().PadLeft(9, '0');
                else
                    return null;
            }
        }

        #region Constructors
        
        public SocialSecurityNumber(string value) 
            : base(value, true) {}

        public SocialSecurityNumber(string value, bool throwExceptions)
            : base(value, throwExceptions) { }

        public SocialSecurityNumber(string formattedSSN = null, string unformattedSSN = null)
            : base(formattedSSN, unformattedSSN, true) { }
        #endregion

        #region Overrides
        
        protected override void ValidateParameters(string formattedValue = null, string unformattedValue = null, bool throwExceptions = false)
        {

            base.ValidateParameters(formattedValue, unformattedValue, throwExceptions);

            if (unformattedValue != null && unformattedValue.Length < 7)
            {
                isParameterSetValid = false;
                if (throwExceptions)
                    throw new ArgumentOutOfRangeException("Unformatted Social Security Number, must contain 7 to 9 integers (allowing for 2 leading zeros.)");
            }

            if (formattedValue != null && formattedValue.Length < 9)
            {
                isParameterSetValid = false;
                if (throwExceptions)
                    throw new ArgumentOutOfRangeException("Formatted Social Security Number, must contain 9 to 11 characters (allowing for 2 leading zeros.)");
            }
        }

        protected override string ApplyFormatting(string value)
        {
            return Format(value);
            //value = value.PadLeft(9, '0');
            //return string.Format("{0}-{1}-{2}", value.Substring(0, 3), value.Substring(3, 2), value.Substring(5, 4));
        }

        protected override string RemoveFormatting(string value)
        {
            value = value.PadLeft(11, '0');
            return Filter(value, char.IsNumber);
        }

        protected override bool isFormatted(string value)
        {
            return value.Contains("-");
        }

        public override string ToString()
        {
            return this.isValid ? this.WithFormatting : null;
        }
        #endregion

        #region Static Methods

        public static explicit operator SocialSecurityNumber(string value)
        {
            return value == null ? null : new SocialSecurityNumber(value);
        }

        public static implicit operator string(SocialSecurityNumber value)
        {
            return value == null ? null : value.WithFormatting;
        }


        // ToDo Change this to a parameter WithoutSecurityEncoding
        public static SocialSecurityNumber Decode(string value, bool throwExceptions = false)
        {

            if (value == null)
                return null;

            Int64 number;

            value = Filter(value, char.IsDigit);

            if (Int64.TryParse(value, out number))
                return new SocialSecurityNumber((number ^ 666666666).ToString(), throwExceptions);
            else
                if (throwExceptions)
                    throw new ArgumentOutOfRangeException("Unable to Decode provided Value.");
                else
                    return null;
        }

        // ToDo: fix these two Decode variations
        public static string DecodeString(string value)
        {
            SocialSecurityNumber tempSSN = SocialSecurityNumber.Decode(value);

            return tempSSN == null ? null : tempSSN.WithoutFormatting;
        }

        public static string Format(string value)
        {
            if (value.IsNullOrEmpty())
                return null;

            value = value.PadLeft(9, '0');
            return string.Format("{0}-{1}-{2}", value.Substring(0, 3), value.Substring(3, 2), value.Substring(5, 4));
        }
        #endregion
    }
}
