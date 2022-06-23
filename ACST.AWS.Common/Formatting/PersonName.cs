//***************************************************************************************
//* FILE:  MedicalClaim.cs
//* 
//***************************************************************************************
//* MODIFICATION LOG:
//* 
//* WHO                      WHEN        WHAT
//* -----------------------  ----------  ------------------------------------------------
//* Brian E. Moore                       Created
//* Neal D. Jarrett          5-7-2015    Rework for MiddleName
//***************************************************************************************

namespace ACST.AWS.Common
{
    using System;
    using System.Linq;

    /// <summary>
    /// Represents a persons name. Providing standard formatting and constituent component parsing.
    /// </summary>
    public class PersonName
    {
        public enum NameOrder
        {
            FirstNameFirst,
            LastNameFirst
        }

        #region Fields & Properties

        readonly string Period = ".";
        readonly string Comma = ",";
        readonly string Seperator = " ";

        string firstName;
        string lastName;
        string middleName;
        string prefix;
        string suffix;

        public string FirstName 
        { 
            get 
            {
                return firstName;
            }
            set
            {
                firstName = value.SafeTrim();
            }
        }

        public string LastName
        {
            get
            {
                return lastName;
            }
            set
            {
                lastName = value.SafeTrim();
            }
        }

        public string MiddleName
        {
            get
            {
                return middleName;
            }
            set
            {
                middleName = value.SafeTrim();
            }
        }

        public string Prefix
        {
            get
            {
                return prefix;
            }
            set
            {
                prefix = value.SafeTrim();
            }
        }

        public string Suffix
        {
            get
            {
                return suffix;
            }
            set
            {
                suffix = value.SafeTrim();
            }
        }

        //public bool UseProperCase { get; set; }
        #endregion

        #region Constructors

        public PersonName() 
        {
        }

        public PersonName(string fullName)
        {
            if (fullName.IsNullOrEmpty())
                throw new ArgumentNullException("fullName");

            ParseFullName(fullName);
        }

        public PersonName(string lastName, string firstName)
            : this(lastName, firstName, null, null, null) { }

        public PersonName(string lastName, string firstName, string middleName, string prefix, string suffix)
        {
            if (lastName.IsNullOrEmpty())
                throw new ArgumentNullException("LastName");

            this.FirstName = firstName;
            this.LastName = lastName;
            this.MiddleName = middleName;
            this.Prefix = prefix;
            this.Suffix = suffix;
        }
        #endregion

        public string FullName(NameOrder order = NameOrder.FirstNameFirst)
        {
            if (order == NameOrder.LastNameFirst)
                return LastNameFirst();
            else
                return FirstNameFirst();
        }

        protected string FirstNameFirst()
        {
            //if (LastName.IsNullOrEmpty() & FirstName.IsNullOrEmpty())
            //    return null;
            //return string.Concat(FirstName, Seperator, LastName);

            string concatName = null;

            if (!Prefix.IsNullOrEmpty())
                concatName = string.Concat(Prefix);

            if (!FirstName.IsNullOrEmpty())
                concatName = string.Concat(concatName, Seperator, FirstName);

            if (!MiddleName.IsNullOrEmpty())
                concatName = string.Concat(concatName, Seperator, MiddleName, Period);

            if (!LastName.IsNullOrEmpty())
                concatName = string.Concat(concatName, Seperator, LastName);

            if (!Suffix.IsNullOrEmpty())
                concatName = string.Concat(concatName, Comma, Seperator, Suffix);

            return concatName.Trim();
        }

        protected string LastNameFirst()
        {
            //if (LastName.IsNullOrEmpty() & FirstName.IsNullOrEmpty())
            //    return null;
            //if (LastName.IsNullOrEmpty())
            //    return FirstName;
            //if (FirstName.IsNullOrEmpty())
            //    return LastName;
            //string fullLast = lastName;
            //if (LastName.IsNotNullOrEmpty() & suffix.IsNotNullOrEmpty())
            //    fullLast = fullLast + " " + suffix;
            //return string.Concat(fullLast, Comma, Seperator, FirstName);

            string concatName = null;

            if (!LastName.IsNullOrEmpty())
                concatName = string.Concat(LastName);

            if (!Suffix.IsNullOrEmpty())
                concatName = string.Concat(concatName, Comma, Seperator, Suffix);

            if (!Prefix.IsNullOrEmpty())
                concatName = string.Concat(concatName, Comma, Seperator, Prefix);

            if (!FirstName.IsNullOrEmpty()) 
                if (Prefix.IsNullOrEmpty())
                    concatName = string.Concat(concatName, Comma, Seperator, FirstName);
                else
                    concatName = string.Concat(concatName, Seperator, FirstName);

            if (!MiddleName.IsNullOrEmpty())
                if (FirstName.IsNullOrEmpty() & Prefix.IsNullOrEmpty())
                    concatName = string.Concat(concatName, Comma, Seperator, MiddleName, Period);
                else
                    concatName = string.Concat(concatName, Seperator, MiddleName, Period);

            return concatName.Trim();
        }

        protected virtual void ParseFullName(string fullName)
        {
            throw new NotImplementedException();
        }

        //public static string Build(string lastName, string firstName, NameOrder order = NameOrder.FirstNameFirst)
        //{
        //    return Build(lastName, firstName, null, null, null, order);
        //}

        public static string Build(string lastName, string firstName, string middleName = null, string prefix = null, string suffix = null, NameOrder order = NameOrder.FirstNameFirst)
        {
            PersonName name = new PersonName(lastName, firstName, middleName, prefix, suffix);
            return name.FullName(order);
        }

        public string ToString(NameOrder order = NameOrder.FirstNameFirst)
        {
            return this.FullName(order);
        }
    }
}
