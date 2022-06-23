
namespace ACST.AWS.TextractClaimMapper.OCR
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using ACST.AWS.Textract.Model;
    using ACST.AWS.Common;
    using ACST.AWS.Common.OCR;
    using System.Diagnostics;

    public static class Mapper
    {
        public static bool SuppressUnMatchedFieldExceptions { get; set; }

        static Mapper()
        {
            SuppressUnMatchedFieldExceptions = true;
        }

        public static bool TableValueMapping<T>(T entity, Table table, int index)
        {
            if (table == null)
                throw new ArgumentNullException(nameof(table));

            //var type = entity.GetType();

            var properties = entity.GetType().GetProperties()
                        .Where(p => Attribute.IsDefined(p, typeof(MappedPropertyAttribute)));

            Logger.TraceVerbose(Environment.NewLine + $"Reading properties for '{entity.GetType().Name}{(index > 0 ? "." + index.ToString() : string.Empty)}' from AWS Textract 'Table.Rows'.");

            //int totalRows = table.Rows.Count;
            //int totalColumns = table.Rows[index].Cells.Count;

            bool hasClaimData = false;

            foreach (var property in properties)
            {
                var attributes = property.GetCustomAttributes(true);
                var mapping = attributes
                                .Where(a => a.GetType().BaseType == typeof(MappedPropertyAttribute));
                //var m2 = mapping.Cast<TableColumnPositionalValueAttribute>().OrderBy(m => m.Column);

                foreach (var m in mapping)
                {
                    var mapsTo = m as TableColumnPositionalValueAttribute;
                    //Logger.TraceVerbose($"MapsTo: {mapsTo.Column}");
                    
                    if(mapsTo.Name.IsNotNullOrEmpty())
                    {
                        var cells = table.Rows[index].Cells.Where(c => c.Match == mapsTo.Name);

                        cells.ToList().ForEach(c => Logger.TraceVerbose($"[{index}, {c.ColumnIndex}] {c.Text} {c.Match}"));

                        var cell = cells.FirstOrDefault();
                        
                        Cell mappedValue = null;
                        
                        if (cell != null)
                        {
                            switch (property.PropertyType.Name)
                            {
                                case "String":
                                    if (cell.Text.IsNotNullOrWhiteSpace())
                                    {
                                        cell.MappedToClaim = true;
                                        cell.Required = mapsTo.Required;
                                        //property.SetValue(entity, cell.Text.SafeTrim()?.ToUpper());
                                        property.SetValue(entity, cell.Text.SafeTrim());
                                        mappedValue = cell;
                                        hasClaimData = true;
                                    }
                                    break;
                                case "Cell":
                                    if (cell.Text.IsNotNullOrWhiteSpace())
                                    {
                                        cell.MappedToClaim = true;
                                        cell.Required = mapsTo.Required;
                                    }
                                    property.SetValue(entity, cell);
                                    mappedValue = cell;
                                    break;
                            }
                        }

                        Logger.TraceVerbose($"the {property.Name.SafeSubstring(0, 15).PadLeft(15, '.')} property maps to the {mapsTo.Name.Replace("ServiceLine", "sl").SafeSubstring(0, 15).PadLeft(15, '.')} at [{index},{cell?.ColumnIndex}]...target found: {cell != null} mappedValue: {mappedValue?.Text.SafeTrim()}");
                    }
                    else
                    {
                        Logger.TraceVerbose($"the {property.Name.SafeSubstring(0, 15).PadLeft(15, ' ')} property maps to the {mapsTo.Name.Replace("ServiceLine", "sl").SafeSubstring(0, 15).PadLeft(15, ' ')} at [{index}, ???]...Outside of column index.");
                    }
                }
            }

            InvokeCompositeValueParser<T>(entity);

            return hasClaimData;
        }

        //public static bool TableValueMapping<T>(T entity, Table table, int index)
        //{
        //    if (table == null)
        //        throw new ArgumentNullException(nameof(table));

        //    var type = entity.GetType();

        //    var properties = entity.GetType().GetProperties()
        //                .Where(p => Attribute.IsDefined(p, typeof(MappedPropertyAttribute)));

        //    Logger.TraceVerbose(Environment.NewLine + $"Reading properties for '{type.Name}{(index > 0 ? "." + index.ToString() : string.Empty)}' from AWS Textract 'Table.Rows'.");

        //    int totalRows = table.Rows.Count;
        //    int totalColumns = table.Rows[index].Cells.Count;

        //    bool hasClaimData = false;
        //    int idealColumn, actualColumn;
        //    int columnOffset = properties.Count() - 11;

        //    foreach (var property in properties)
        //    {
        //        var attributes = property.GetCustomAttributes(true);
        //        var mapping = attributes
        //                        .Where(a => a.GetType().BaseType == typeof(MappedPropertyAttribute));

        //        foreach (var m in mapping)
        //        {
        //            var mapsTo = m as TableColumnPositionalValueAttribute;


        //            idealColumn = mapsTo.Column;
        //            actualColumn = idealColumn + columnOffset;

        //            if (actualColumn < totalColumns)
        //            {
        //                var cell = table.Rows[index].Cells[actualColumn];

        //                if (cell != null)
        //                {
        //                    switch (property.PropertyType.Name)
        //                    {
        //                        case "String":
        //                            if (cell.Text.IsNotNullOrWhiteSpace())
        //                            {
        //                                if (actualColumn > 0)
        //                                    cell.MappedToClaim = true;
        //                                property.SetValue(entity, cell.Text.SafeTrim());
        //                            }
        //                            break;
        //                        case "Cell":
        //                            if (actualColumn > 0 & cell.Text.IsNotNullOrWhiteSpace())
        //                                cell.MappedToClaim = true;
        //                            property.SetValue(entity, cell);
        //                            break;
        //                    }

        //                    hasClaimData = hasClaimData | cell.MappedToClaim;
        //                }

        //                Logger.TraceVerbose($"the {property.Name} property maps to the {mapsTo.Name} at [{index},{actualColumn}]...target found: {cell != null}");
        //            }
        //            else
        //            {
        //                Logger.TraceVerbose($"the {property.Name} property maps to the {mapsTo.Name} at [{index},{actualColumn}]...Outside of column index.");
        //            }
        //        }
        //    }

        //    InvokeCompositeValueParser<T>(entity);

        //    return hasClaimData;
        //}

        public static void MatchedElementMapping<T, TM>(T entity, List<TM> matchedElementsList, int index = 0) where T
            : new() where TM : IMatchedElement, IMappedElement, IAWSElement
        {
            bool parseCompositeValues = true;

            var type = entity.GetType();

            if (!matchedElementsList.Any())
            {
                Logger.TraceVerbose(Environment.NewLine + $"Skipping '{type.Name}{(index > 0 ? "." + index.ToString() : string.Empty)}' property map, source AWS Textract collection is empty.");
                return;
            }

            var properties = entity.GetType().GetProperties()
                        .Where(p => Attribute.IsDefined(p, typeof(MappedPropertyAttribute)));

            Logger.TraceVerbose(Environment.NewLine + $"Reading properties for '{type.Name}{(index > 0 ? "."+index.ToString() : string.Empty)}' from AWS Textract '{matchedElementsList?.First().GetType().GetTypeInfo().Name}s'.");

            foreach (var property in properties)
            {
                //if (property.Name == "ServicesTotalFee")
                //    Debugger.Break();

                var attributes = property.GetCustomAttributes(true);
                var mapping = attributes
                                .Where(a => a.GetType().InheritsFrom(typeof(MappedPropertyAttribute))); 
                
                foreach (var m in mapping)
                {
                    var mapsTo = m as MappedPropertyAttribute;
                    
                    var matchedElements = matchedElementsList.Where(e => e.Match == mapsTo.Name);
                    bool matched = matchedElements.Any();
                    bool required = mapsTo.Required;

                    if (!matched && required)
                    {
                        if (SuppressUnMatchedFieldExceptions)
                            Logger.TraceWarning($"Required property '{type.GetTypeInfo().Name}.{mapsTo.Name}', not found in AWS Textract element '{(matchedElements.Any() ? matchedElements?.First().GetType().GetTypeInfo().Name : "nomatch")}'.");
                        else
                            throw new RequiredPropertyException(mapsTo.Name, type.GetTypeInfo().Name, matchedElements?.First().GetType().GetTypeInfo().Name);
                    }

                    if (matchedElements.Count() > 1)
                    {
                        var t = matchedElements.Select(i => i.Text).Aggregate((i, j) => i + ", " + j);
                        Logger.TraceWarning($"Multiple elements match named coords: {t}");
                    }
                    else if (matched)
                    {
                        var matchedElement = matchedElements.SingleOrDefault();
                        bool updatedByUser = matchedElement?.UpdateToClaim ?? false;
                        parseCompositeValues = parseCompositeValues & !updatedByUser;

                        switch (property.PropertyType.Name)
                        {
                            case "Bool":
                                Logger.TraceWarning($"Bool Property '{mapsTo.Name}'.");
                                break;
                            //case "Decimal":
                            //    if (matchedElement.SingleOrDefault() != null)
                            //    {
                            //        property.SetValue(entity, matchedElement.Single().Text);
                            //        matchedElement.Single().MappedToClaim = true;
                            //    }
                            //    else
                            //    {
                            //        if (mapsTo.Required)
                            //            if (SuppressUnMatchedFieldExceptions)
                            //                Logger.TraceWarning($"UnMatched Required Property '{mapsTo.Name}'.");
                            //            else
                            //                throw new RequiredPropertyException(mapsTo.Name);
                            //    }
                            //    break;
                            case "Decimal":
                            case "String":
                                if (matchedElement != null & !updatedByUser)
                                {
                                    //property.SetValue(entity, matchedElement.Text?.ToUpper());
                                    property.SetValue(entity, matchedElement.Text);
                                    matchedElement.MappedToClaim = true;
                                    matchedElement.Required = required;
                                }
                                else
                                {
                                    if (mapsTo.Required & !updatedByUser)
                                        if (SuppressUnMatchedFieldExceptions)
                                            Logger.TraceWarning($"UnMatched Required Property '{mapsTo.Name}'.");
                                        else
                                            throw new RequiredPropertyException(mapsTo.Name);
                                }
                                break;
                            case "Field":
                                if (mapsTo.Required && matchedElement == null)
                                {
                                    if (mapsTo.Required)
                                        if (SuppressUnMatchedFieldExceptions)
                                            Logger.TraceWarning($"UnMatched Required Property '{mapsTo.Name}'.");
                                        else
                                            throw new RequiredPropertyException(mapsTo.Name);
                                }
                                else
                                {
                                    property.SetValue(entity, matchedElement);
                                    matchedElement.MappedToClaim = true;
                                    matchedElement.Required = required;

                                }
                                break;
                            case "Line":
                                property.SetValue(entity, matchedElement);
                                break;
                        }
                    }
                    Logger.TraceVerbose($"the {property.Name} property maps to the {mapsTo.Name} target_property...target found: {matched}");
                }
            }

            if(parseCompositeValues)
                InvokeCompositeValueParser<T>(entity);
        }

        public static void InvokeCompositeValueParser<T>(T entity)
        {
            var type = entity.GetType();

            MethodInfo methodInfo = type.GetMethod("ParseCompositeValues");

            // Invoke by name, otherwise find by attribute then invoke composite parser
            if (methodInfo != null)
            {
                var result = methodInfo.Invoke(entity, null);
            }
            else
            {
                var methods = entity.GetType().GetMethods();
                foreach (var method in methods)
                {
                    var attributes = method.GetCustomAttributes(true);
                    var parser = attributes
                                    .FirstOrDefault(a => a.GetType() == typeof(CompositeValueParserAttribute));
                    if (parser != null)
                    {
                        method.Invoke(entity, null);
                    }
                }
            }

        }

        //These don't work
        //public static IEnumerable<PropertyInfo> FindPropertiesByCompositeAttributeName(this Type type, string name = null)
        //{
        //    //return FindPropertiesByCustomAttribute(type, typeof(OCR.CompositeValueAttribute), name, null);
        //    return GetMappedProperties(type, typeof(OCR.CompositeValueAttribute))
        //        .Where(p => ((IMappedPropertyAttribute)p).GroupName == name);
        //}

        //public static IEnumerable<PropertyInfo> FindPropertiesByGroupFieldAttributeName(this Type type, string name = null)
        //{
        //    //return FindPropertiesByCustomAttribute(type, typeof(OCR.GroupFieldAttribute), null, name);
        //    return GetMappedProperties(type, typeof(OCR.GroupFieldAttribute))
        //        .Where( p => ((IMappedPropertyAttribute)p).GroupName == name);
        //}

        public static IEnumerable<PropertyInfo> GetMappedProperties(this Type type, Type attribute)
        {
            return from p in type?.GetProperties()
                                  .Select(p => GetCustomProperty(type, p.Name))
                                  .Where(p => p != null & Attribute.IsDefined(p, attribute))
                   from c in p.GetCustomAttributes(true)
                   where c.GetType().InheritsFrom(typeof(IMappedPropertyAttribute))
                   select p;
        }

        public static IEnumerable<PropertyInfo> FindPropertiesByCompositeAttributeName(this Type type, string name = null)
        {
            return FindPropertiesByCustomAttribute(type, typeof(CompositeValueAttribute), name, null);
        }

        public static IEnumerable<PropertyInfo> FindPropertiesByGroupFieldAttributeName(this Type type, string name = null)
        {
            return FindPropertiesByCustomAttribute(type, typeof(GroupFieldAttribute), null, name);
        }

        static IEnumerable<PropertyInfo> FindPropertiesByCustomAttribute(this Type type, Type attribute, string attributeName = null, string attributeGroupName = null)
        {
            var properties = from p in type?.GetProperties()
                                  .Select(p => GetCustomProperty(type, p.Name))
                                  .Where(p => p != null & Attribute.IsDefined(p, attribute))
                             from c in p.GetCustomAttributes(true)
                             where c.GetType().InheritsFrom(typeof(IMappedPropertyAttribute))
                                    && (
                                            attributeName == null |
                                            ((IMappedPropertyAttribute)c).Name == attributeName
                                        )
                                    && (
                                            attributeGroupName == null |
                                            ((IMappedPropertyAttribute)c).GroupName == attributeGroupName
                                        )
                             select p;
            //.Where(a => a.GetType().InheritsFrom(typeof(MappedPropertyAttribute)));
            return properties;
        }

        static IEnumerable<PropertyInfo> GetCustomProperties(Type type, Type customAttribute)
        {
            return type?.GetProperties()
                          .Select(p => GetCustomProperty(type, p.Name))
                          .Where(p => p != null & Attribute.IsDefined(p, customAttribute));
        }

        static PropertyInfo GetCustomProperty(Type type, string name)
        {
            if (type == null)
                return null;

            var prop = type.GetProperty(name);

            if (prop.DeclaringType == type)
                return prop;
            else
                return GetCustomProperty(type.BaseType, name);
        }

        public static T GetCustomPropertyCustomAttribute<T>(Type type, string name) 
            where T : class
        {
            var propAttr = OCR.Mapper.GetCustomProperty(type, name);
            var custPropAttrs = propAttr.GetCustomAttributes(true); 
            var custPropAttr = custPropAttrs.Where(a => a.GetType() == typeof(CompositeValueAttribute));
            var customPropertyCustomAttribute = custPropAttr.SingleOrDefault() as T;

            return customPropertyCustomAttribute;
        }

        public static IEnumerable<T> GetCustomPropertiesCustomAttribute<T>(Type type)
            where T : class
        {
            IEnumerable<T> ret = GetCustomProperties(type, typeof(T))
                //.Select(a => new { Prop = a, Attr = a.GetCustomAttributes(true).FirstOrDefault() });
                .Select(a => a.GetCustomAttributes(true).FirstOrDefault()).Cast<T>();

            return ret;
        }

        public static IEnumerable<CompositeValuePropertyItem> GetCompositeValuePropertyItems(Type type)
        {
            IEnumerable<CompositeValuePropertyItem> ret = GetCustomProperties(type, typeof(CompositeValueAttribute))
                .Select(a => new CompositeValuePropertyItem { PropertyInfo = a, Attribute = (CompositeValueAttribute)a.GetCustomAttributes(true).FirstOrDefault() });
                //.Select(a => a.GetCustomAttributes(true).FirstOrDefault()).Cast<T>();

            return ret;
        }

        //public static MappedPropertyAttributeItem FindPropertyAttributeItemByMappedAttributeName(this Type type, string attributeName)
        //{
        //    foreach (PropertyInfo propertyInfo in type.GetProperties())
        //    {
        //        object[] mappedAttributes = propertyInfo.GetCustomAttributes(typeof(MappedPropertyAttribute), false);

        //        foreach (MappedPropertyAttribute attribute in mappedAttributes)
        //        {
        //            if (attribute.Name == attributeName)
        //            {
        //                return new MappedPropertyAttributeItem { PropertyInfo = propertyInfo, Attribute = attribute };
        //            }
        //        }
        //    }
        //    return null;
        //}


        public static PropertyInfo FindPropertyByMappedAttributeName(this Type type, string attributeName)
        {
            foreach (PropertyInfo propertyInfo in type.GetProperties())
            {
                object[] mappedAttributes = propertyInfo.GetCustomAttributes(typeof(MappedPropertyAttribute), false);

                foreach (MappedPropertyAttribute attribute in mappedAttributes)
                {
                    if (attribute.Name == attributeName)
                    {
                        return propertyInfo;
                    }
                }
            }
            return null;
        }



        //// Works but not used
        //public static IEnumerable<PropertyInfo> FindRequiredProperties(this Type type)
        //{
        //    var props = type?.GetProperties();

        //    foreach (PropertyInfo propertyInfo in props)
        //    {
        //        object[] mappedAttributes = propertyInfo.GetCustomAttributes(typeof(MappedPropertyAttribute), false);

        //        foreach (MappedPropertyAttribute attribute in mappedAttributes)
        //        {
        //            if (attribute.Required == true)
        //            {
        //                yield return propertyInfo;
        //            }
        //        }
        //    }
        //}

        //// Works but not used
        //public static IEnumerable<PropertyInfo> FindCompositeProperties(this Type type, string name)
        //{
        //    var props = type?.GetProperties();

        //    foreach (PropertyInfo propertyInfo in props)
        //    {
        //        object[] mappedAttributes = propertyInfo.GetCustomAttributes(typeof(CompositeValueAttribute), false);

        //        foreach (IMappedPropertyAttribute attribute in mappedAttributes)
        //        {
        //            if (attribute.Name == name)
        //            {
        //                yield return propertyInfo;
        //            }
        //        }
        //    }
        //    //return null;
        //}



        // ---------------------------------------

        public static void GridValueMapping<T>(T entity, List<MatchedLine> lines, int rowId, int gridRowOffset = 0)
        {

            var type = entity.GetType();

            var properties = entity.GetType().GetProperties()
                        .Where(p => Attribute.IsDefined(p, typeof(MappedPropertyAttribute)));

            //var orderedProperties = properties
            //    .OrderBy(p => p.GetCustomAttributes().OfType<ColumnPositionalValueAttribute>().First().Column);


            Logger.TraceInfo(Environment.NewLine + $"Finding properties for {type.Name}");

            int firstRow, targetRow;
            //int rowOffset = 0;
            //int idealColumn, actualColumn;
            //int columnOffset = properties.Count() - 11;

            foreach (var property in properties)
            {
                var attributes = property.GetCustomAttributes(true);
                var mapping = attributes
                                .Where(a => a.GetType().BaseType == typeof(TableColumnPositionalValueAttribute));

                //Logger.TraceVerbose( $"Total Attribute count: {attributes.Count()}, mapping: {mapping.Count()}");

                foreach (var m in mapping)
                {
                    var mapsTo = m as GridPositionalValueAttribute;

                    firstRow = mapsTo.Row + gridRowOffset;
                    targetRow = firstRow + rowId;

                    var line = lines.Where(l => l.Geometry.ReadingOrder.Column >= mapsTo.Column
                                                & l.Geometry.ReadingOrder.Column <= mapsTo.Column + mapsTo.Width
                                                & l.Geometry.ReadingOrder.Row == targetRow).FirstOrDefault();

                    if (line != null)
                    {
                        switch (property.PropertyType.Name)
                        {
                            case "String":
                                if (line.Text.IsNotNullOrWhiteSpace())
                                    property.SetValue(entity, line.Text);
                                break;
                            case "Line":
                                property.SetValue(entity, line);
                                break;
                        }

                    }
                    Logger.TraceVerbose($"the {property.Name} property maps to the {mapsTo.Name} at [{targetRow},{mapsTo.Column}]...target found: {line != null}");
                }
            }

            MethodInfo methodInfo = type.GetMethod("ParseCompositeValues");

            // Invoke by name, otherwise find by attribute then invoke composite parser
            if (methodInfo != null)
            {
                var result = methodInfo.Invoke(entity, null);
            }
            else
            {
                var methods = entity.GetType().GetMethods();
                foreach (var method in methods)
                {
                    var attributes = method.GetCustomAttributes(true);
                    var parser = attributes
                                    .FirstOrDefault(a => a.GetType() == typeof(CompositeValueParserAttribute));
                    if (parser != null)
                    {
                        method.Invoke(entity, null);
                    }
                }
            }
        }

        public static void KeyValueMapping<T>(T entity, Dictionary<string, Field> matchedFieldMap) where T
            : new()
        {
            var type = entity.GetType();
            //var properties = entity.GetType().GetProperties();

            var properties = entity.GetType().GetProperties()
                        .Where(p => Attribute.IsDefined(p, typeof(MappedPropertyAttribute)));

            Logger.TraceInfo(Environment.NewLine + $"Finding properties for {type.Name}");

            var x = properties.Count();

            foreach (var property in properties)
            {
                var attributes = property.GetCustomAttributes(true);
                var mapping = attributes
                                .Where(a => a.GetType().BaseType == typeof(MappedPropertyAttribute));

                //Logger.TraceVerbose( $"Total Attribute count: {attributes.Count()}, mapping: {mapping.Count()}");

                foreach (var m in mapping)
                {
                    var mapsTo = m as MappedPropertyAttribute;

                    bool matched = matchedFieldMap.ContainsKey(mapsTo.Name);
                    bool required = mapsTo.Required;

                    if (!matched && required)
                        throw new RequiredPropertyException(mapsTo.Name);

                    if (matched)
                    {
                        switch (property.PropertyType.Name)
                        {
                            case "String":
                                if (matchedFieldMap[mapsTo.Name].Value != null)
                                    property.SetValue(entity, matchedFieldMap[mapsTo.Name].Value.Text);
                                else
                                {
                                    if (mapsTo.Required)
                                        throw new RequiredPropertyException(mapsTo.Name);
                                }
                                break;
                            case "FieldValue":
                                if (mapsTo.Required && matchedFieldMap[mapsTo.Name].Value == null)
                                {
                                    throw new RequiredPropertyException(mapsTo.Name);
                                }
                                else
                                    property.SetValue(entity, matchedFieldMap[mapsTo.Name].Value);
                                break;
                        }

                    }
                    Logger.TraceVerbose($"the {property.Name} property maps to the {mapsTo.Name} target_property...target found: {matched}");
                }
            }

            MethodInfo methodInfo = type.GetMethod("ParseCompositeValues");

            // Invoke by name, otherwise find by attribute then invoke composite parser
            if (methodInfo != null)
            {
                var result = methodInfo.Invoke(entity, null);
            }
            else
            {
                var methods = entity.GetType().GetMethods();
                foreach (var method in methods)
                {
                    var attributes = method.GetCustomAttributes(true);
                    var parser = attributes
                                    .FirstOrDefault(a => a.GetType() == typeof(CompositeValueParserAttribute));
                    if (parser != null)
                    {
                        method.Invoke(entity, null);
                    }
                }
            }
        }

    }
}
