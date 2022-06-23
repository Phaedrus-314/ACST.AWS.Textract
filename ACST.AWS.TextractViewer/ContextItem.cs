
namespace ACST.AWS.TextractViewer
{
    using System.Reflection;

    public enum ContextItemColumnIndex
    {
        Required = 0,
        Name = 1,
        Value = 2,
        Type = 3,
        PropertyInfo = 4,
        Group = 5
    }


    public class ContextItem
    {
        public bool Required { get; set; }

        public string Group { get; set; }

        public string Name { get; set; }

        public object Value { get; set; }

        public string Type { get; set; }

        public PropertyInfo MappedPropertyInfo { get; set; }
    }
}
