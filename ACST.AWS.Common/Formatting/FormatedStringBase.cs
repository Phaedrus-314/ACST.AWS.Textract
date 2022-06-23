
namespace ACST.AWS.Common
{
    using System;
    using System.Diagnostics;
    using System.Linq;

    /// <summary>
    /// Formatted String Base Class
    /// </summary>
    [DebuggerStepThrough()]
    public abstract class FormattedStringBase
    {
        #region Properties

        public string OriginalValue { get; private set; }

        public bool isOriginalValueFormatted { get; private set; }

        protected bool isParameterSetValid { get; set; }

        public string WithFormatting { get; protected set; }

        public string WithoutFormatting { get; protected set; }

        public virtual bool isValid
        {
            get
            {
                return ( 
                        isParameterSetValid 
                        && (    ApplyFormatting(WithoutFormatting) == WithFormatting
                                && 
                                RemoveFormatting(WithFormatting) == WithoutFormatting
                            )
                       );
            }
        }
        #endregion

        #region Constructors

        public FormattedStringBase(string value, bool throwExceptions = false)
        {
            isParameterSetValid = true;

            if (value.IsNullOrEmpty())
            {
                OriginalValue = value;
                isParameterSetValid = false;
                isOriginalValueFormatted = false;
                if (throwExceptions)
                    throw new ArgumentNullException("Value");
                return;
            }

            if (isFormatted(value))
            {
                ValidateParameters(formattedValue: value, throwExceptions: throwExceptions);
                
                Initialize(formattedValue: value);
            }
            else
            {
                ValidateParameters(unformattedValue: value, throwExceptions: throwExceptions);
                
                Initialize(unformattedValue: value);
            }
        }

        public FormattedStringBase(string formattedValue = null, string unformattedValue = null, bool throwExceptions = false)
        {
            isParameterSetValid = true;

            ValidateParameters(formattedValue, unformattedValue, throwExceptions);
            
            Initialize(formattedValue, unformattedValue);
        }

        #endregion

        #region Abstract & Virtual Methods

        protected virtual void ValidateParameters(string formattedValue = null, string unformattedValue = null, bool throwExceptions = false)
        {
            if (formattedValue.IsNullOrEmpty() && unformattedValue.IsNullOrEmpty())
            {
                isParameterSetValid = false;
                if (throwExceptions)
                    throw new ArgumentNullException("At least one parameter must contain a non-null value.");
            }
        }

        protected abstract string ApplyFormatting(string value);

        protected abstract string RemoveFormatting(string value);

        protected abstract bool isFormatted(string value);
        #endregion

        #region Private Methods

        void Initialize(string formattedValue = null, string unformattedValue = null)
        {
            if (unformattedValue.IsNotNullOrEmpty())
            {
                OriginalValue = unformattedValue;
                isOriginalValueFormatted = false;

                WithFormatting = ApplyFormatting(unformattedValue);
                WithoutFormatting = RemoveFormatting(WithFormatting);
            }
            else if (formattedValue.IsNotNullOrEmpty())
            {
                OriginalValue = formattedValue;
                isOriginalValueFormatted = true;

                WithoutFormatting = RemoveFormatting(formattedValue);
                WithFormatting = ApplyFormatting(WithoutFormatting);
            }
        }
        #endregion

        #region Overrides

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            FormattedStringBase s = obj as FormattedStringBase;
            if ((Object)s == null)
                return false;

            //return (this.WithFormatting == s.WithFormatting)
            //        &&
            //        (this.isValid == s.isValid);

            return Equals(s);
        }

        public bool Equals(FormattedStringBase s)
        {
            if ((Object)s == null)
                return false;

            return (this.WithFormatting == s.WithFormatting)
                    &&
                    (this.isValid == s.isValid);
        }

        public override int GetHashCode()
        {
            int WithFormattingHash =
                this.WithFormatting == null ? 0 : this.WithFormatting.GetHashCode();
            return this.isValid.GetHashCode() ^ WithFormattingHash;
        }

        #endregion

        #region Operators

        public static bool operator ==(FormattedStringBase a, FormattedStringBase b)
        {

            if (Object.ReferenceEquals(a, b))
                return true;

            if((((object)a ==null || (object)b == null)))
                return false;

            return a.isValid == b.isValid && a.WithFormatting == b.WithFormatting;
        }

        public static bool operator !=(FormattedStringBase a, FormattedStringBase b)
        {
            return !(a == b);
        }
        #endregion

        /// <summary>
        /// Return only those characters specified by the filter.
        /// </summary>
        /// <param name="text">Source string.</param>
        /// <param name="filter">Function specifing which characters to return.</param>
        /// <returns>Source string filtered by supplied funcion.</returns>
        [DebuggerStepThrough]
        public static string Filter(string text, Func<char, bool> filter)
        {
            if (text == null)
                return null;

            return new String(text.Where(filter).ToArray());
        }
    }
}
