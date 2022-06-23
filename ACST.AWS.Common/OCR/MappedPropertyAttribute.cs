
namespace ACST.AWS.Common.OCR
{
    using System;
    using System.Reflection;

    [System.Diagnostics.DebuggerDisplay("{ToString()}")]
    public class CompositeValuePropertyItem
    {
        public bool IsRequired => Attribute?.Required ?? false;

        public PropertyInfo PropertyInfo { get; set; }

        public CompositeValueAttribute Attribute { get; set; }

        public override string ToString()
        {
            return $"{PropertyInfo.Name}: IsRequired: {IsRequired}";
        }
    }

    //[System.Diagnostics.DebuggerDisplay("{ToString()}")]
    //public class MappedPropertyAttributeItem
    //{
    //    public bool IsRequired => Attribute?.Required ?? false;

    //    public PropertyInfo PropertyInfo { get; set; }

    //    public MappedPropertyAttribute Attribute { get; set; }

    //    public override string ToString()
    //    {
    //        return $"{PropertyInfo.Name}: IsRequired: {IsRequired}";
    //    }
    //}


    [AttributeUsage(AttributeTargets.Property)]
    public class MappedPropertyAttribute
        : Attribute, IMappedPropertyAttribute
    {
        public string GroupName { get; protected set; }

        public string Name { get; private set; }

        public bool Required { get; private set; }

        public MappedPropertyAttribute(string name)
            : this(name, false) { }

        public MappedPropertyAttribute(string name, bool required)
        {
            this.Name = name;
            this.Required = required;
        }

        public MappedPropertyAttribute(string name, string groupName, bool required = false)
        {
            this.Name = name;
            this.GroupName = groupName;
            this.Required = required;
        }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class PositionalKeyAttribute
        : MappedPropertyAttribute
    {
        public PositionalKeyAttribute(string name) : base(name) { }
    }


    /// <summary>
    /// Identifies OCR Value goupe properties that resolve to claim properties
    /// ie., Several SubscriberGender Field properties resolve to one 
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class GroupFieldAttribute
        : MappedPropertyAttribute
    {
        public GroupFieldAttribute(string name, string groupName, bool required = false)
            : base(name, groupName, required) { }
    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class PositionalValueAttribute
        : MappedPropertyAttribute
    {
        public PositionalValueAttribute(string name) : base(name) { }

        public PositionalValueAttribute(string name, bool required) : base(name, required) { }
    }

    /// <summary>
    /// Identifies the claim component properties that derive from composite OCR values.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class CompositeValueAttribute
        : Attribute, IMappedPropertyAttribute
    {
        public string GroupName { get; private set; }

        public string Name { get; private set; }

        //public bool Required { get { return false; } }
        public bool Required { get; private set; }

        public CompositeValueAttribute(string name)
        {
            this.Name = name;
        }

        public CompositeValueAttribute(string name, bool required)
            : this(name)
        {
            this.Required = required;
        }
        //public CompositeValueAttribute(string name, string groupName) 
        //    : this(name)
        //{
        //    this.GroupName = groupName;
        //}
    }


    //class MyCustomAttribute : Attribute
    //{
    //    public int[] Values { get; set; }

    //    public MyCustomAttribute(params int[] values)
    //    {
    //        this.Values = values;
    //    }
    //}

    //[MyCustomAttribute(3, 4, 5)]

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class TableColumnCompositeValueAttribute
        : CompositeValueAttribute
    {
        public int Column { get; private set; }

        public TableColumnCompositeValueAttribute(string name, int column, bool required = false)
            : base(name, required)
        {
            this.Column = column;
        }
    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class TableColumnPositionalValueAttribute 
        : MappedPropertyAttribute
    {
        public int Column { get; private set; }

        public TableColumnPositionalValueAttribute(string name, int column, bool required = false)
            : base(name, required)
        {
            this.Column = column;
        }
    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class GridPositionalValueAttribute
        : TableColumnPositionalValueAttribute
    {
        public int Row { get; private set; }
        public int Width { get; private set; }

        public GridPositionalValueAttribute(string name, int row, int column, int width = 0)
            : base (name, column)
        {
            this.Row = row;
            this.Width = width;
        }
    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class GridCompositeValueAttribute
        : TableColumnCompositeValueAttribute
    {
        public int Row { get; private set; }
        public int Width { get; private set; }

        public GridCompositeValueAttribute(string name, int row, int column, int width = 0)
            : base(name, column)
        {
            this.Row = row;
            this.Width = width;
        }
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class CompositeValueParserAttribute
    : Attribute
    { }


    //public class ClaimAttribute<T>
    //{
    //    [XmlAttribute]
    //    public T Value { get; set; }
    //}
}
