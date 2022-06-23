
namespace ACST.AWS.Common
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Diagnostics;
    using System.Reflection;

    //[DebuggerStepThrough]
    public static class TypeExtensions
    {
        /// <summary>
        /// Extension method to check the entire inheritance hierarchy of a
        /// type to see whether the given base type is inherited.
        /// </summary>
        /// <param name="type">The Type object this method was called on</param>
        /// <param name="baseType">The base type to look for in the 
        /// inheritance hierarchy</param>
        /// <returns>True if baseType is found somewhere in the inheritance hierarchy, false if not.</returns>
        public static bool InheritsFrom(this Type type, Type baseType)
        {
            // null does not have base type
            if (type == null) 
                return false;

            // only interface or object can have null base type
            if (baseType == null)
                return type.IsInterface || type == typeof(object);

            // check implemented interfaces
            if (baseType.IsInterface)
                return type.GetInterfaces().Contains(baseType);

            // check all base types
            var currentType = type;
            while (currentType != null)
            {
                if (currentType.BaseType == baseType)
                {
                    return true;
                }

                currentType = currentType.BaseType;
            }

            return false;
        }

        public static IEnumerable<Type> GetParentTypes(this Type type)
        {
            // is there any base type?
            if (type == null)
                yield break;

            // return all implemented or inherited interfaces
            foreach (var i in type.GetInterfaces())
                yield return i;

            // return all inherited types
            var currentBaseType = type.BaseType;
            while (currentBaseType != null)
            {
                yield return currentBaseType;
                currentBaseType = currentBaseType.BaseType;
            }
        }
    }

    public static class PropertyInfoExcensions
    {
        public static T CustomAttribute<T>(this PropertyInfo propertyInfo, string attributeName = null)
            where T : OCR.IMappedPropertyAttribute
        {
            object[] mappedAttributes = propertyInfo.GetCustomAttributes(typeof(T), false);

            if (attributeName.IsNullOrWhiteSpace())
                return (T)mappedAttributes.FirstOrDefault();
            else
            {
                foreach (OCR.IMappedPropertyAttribute attribute in mappedAttributes)
                {
                    if (attribute.Name == attributeName)
                    {
                        return (T)attribute;
                    }
                }
            }

            return default(T);
        }
    }
}
