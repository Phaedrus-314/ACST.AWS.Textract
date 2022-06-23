using System;
using System.Collections.Generic;

namespace ACST.AWS.TextractViewer
{
    using System.Drawing;
    using System.Linq;
    using System.Reflection;

    using ACST.AWS.Textract.Model;
    using ADA = TextractClaimMapper.ADA;
    using OCR = TextractClaimMapper.OCR;
    using ACST.AWS.Common;
    using Newtonsoft.Json;
    using ACST.AWS.Common.OCR;
    using System.Net.Http.Headers;

    public struct ContextData
    {

        #region Fields & Properties

        public PointF Coordinate { get; set; }

        public IEnumerable<CompositeValuePropertyItem> CompositeValuePropertyItems { get; set; }

        public IEnumerable<CompositeValuePropertyItem> GroupFieldPropertyItems { get; set; }

        //public IEnumerable<PropertyInfo> ComositeValueProperties { get; set; }

        //public IEnumerable<PropertyInfo> GroupFieldProperties { get; set; }

        public Field Element { get; set; }

        Cell CellElement => CellElements?.SingleOrDefault();

        public string ElementType { get; set; }

        public string PropertyType => MappedPropertyInfo?.PropertyType.Name;

        public IEnumerable<Field> Elements { get; set; }

        public IEnumerable<Cell> CellElements { get; set; }

        public Type MappedPropertAttribute { get; set; }

        public string MappedPropertyGroupName { get; set; }

        public string MappedPropertyAttributeInstanceName { get; set; }

        public string MappedPropertyName => MappedPropertyInfo?.Name;

        public PropertyInfo MappedGroupPropertyInfo { get; set; }

        public PropertyInfo MappedPropertyInfo { get; set; }

        public bool IsFieldRequired { get; set; }

        public bool IsApproved => Element?.UpdateToClaim ?? false;

        public string OCRFieldName => Element?.Key.Text;

        public string OCRFieldValue => Element?.Value?.Text;

        public int OCRFieldKeyConfidence => Convert.ToInt32(Element?.Key.Confidence ?? 0);

        public int OCRFieldValueConfidence => Convert.ToInt32(Element?.Value?.Confidence ?? 0);

        public int OCRFieldMinConfidence => Convert.ToInt32(Element?.MinConfidence ?? 0);

        public string JSONdata { get; set; }

        public TextractElements SelectedElements { get; set; }
        #endregion

        public void SetUpdated(bool value)
        {
            if (this.Element != null)
                this.Element.UpdateToClaim = value;
        }

        public void SetMinimumConfidence(float value)
        {
            if (this.Element != null)
                this.Element.Key.MinConfidence = value;
        }

        //public static ContextData GetContext(TextractClaimImage zonedTextractImage, string fieldName, TextractElements selectedElements)
        //{
        // no easy way from claim property name to unmatched txtract form field
        //}

        public static ContextData GetContext(TextractClaimImage zonedTextractImage, Field field, TextractElements selectedElements)
        {
            // ToDo: Need to unify these GetContext Methods
            string jsonData = null;
            Field element = null;
            string elementType = null;
            IEnumerable<Cell> cellElements = null;

            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.ContractResolver = new DynamicContractResolver(new string[] { "Polygon" });

            if (selectedElements.HasFlag(TextractElements.Field) | selectedElements.HasFlag(TextractElements.FieldKey) | selectedElements.HasFlag(TextractElements.FieldValue))
            {
                elementType = TextractElements.Field.ToString();
                element = field;
                jsonData = JsonConvert.SerializeObject(element, Formatting.Indented, settings);
            }

            //if ((jsonData == "[]" | jsonData == "null") && selectedElements.HasFlag(TextractElements.Table))
            //{
            //    elementType = TextractElements.Table.ToString();
            //    cellElements = zonedTextractImage.Page?.Tables[0].GetCellsByCoordinate(coordinate);
            //    jsonData = JsonConvert.SerializeObject(cellElements, Formatting.Indented, settings);
            //}

            if (element == null & cellElements?.Count() == 0)
                return default(ContextData);

            string mappedPropertyAttributeInstanceName = element?.Match;
            string mappedPropertyGroupName = element?.Key.GroupName;

            PropertyInfo mappedGroupPropertyInfo = OCR.Mapper
                .FindPropertyByMappedAttributeName(typeof(ADA.ADAClaim), mappedPropertyGroupName);

            PropertyInfo mappedPropertyInfo = OCR.Mapper
                .FindPropertyByMappedAttributeName(typeof(ADA.ADAClaim), mappedPropertyAttributeInstanceName);

            // ToDo: Cache all the PropertyInfo values so we don't keep running this reflection
            var compositeValuePropertyItems = OCR.Mapper.GetCompositeValuePropertyItems(typeof(ADA.ADAClaim))
                                                .Where(i => i.Attribute.Name == mappedPropertyAttributeInstanceName);

            //var groupFieldPropertyItems = OCR.Mapper.GetCompositeValuePropertyItems(typeof(ADA.ADAClaim))
            //                                    .Where(i => i.Attribute.GroupName == mappedPropertyGroupName);


            var mappedGroupPropertyCustAttr = mappedGroupPropertyInfo?.CustomAttribute<IMappedPropertyAttribute>();
            var mappedPropertyCustAttr = mappedPropertyInfo?.CustomAttribute<IMappedPropertyAttribute>();

            var mappedPropertAttribute = mappedPropertyCustAttr?.GetType();

            bool isRequired = (mappedPropertyCustAttr?.Required
                    | mappedGroupPropertyCustAttr?.Required
                    | compositeValuePropertyItems.Any(c => c.IsRequired)
                    ) ?? false;

            return new ContextData
            {
                //Coordinate = coordinate,
                CellElements = cellElements,
                ElementType = elementType,
                JSONdata = jsonData,
                SelectedElements = selectedElements,
                Element = element,
                IsFieldRequired = isRequired,
                MappedGroupPropertyInfo = mappedGroupPropertyInfo,
                MappedPropertyGroupName = mappedPropertyGroupName,
                MappedPropertyInfo = mappedPropertyInfo,
                MappedPropertyAttributeInstanceName = mappedPropertyAttributeInstanceName,
                MappedPropertAttribute = mappedPropertAttribute,
                CompositeValuePropertyItems = compositeValuePropertyItems
                //GroupFieldPropertyItems = groupFieldPropertyItems
            };
        }

        public static ContextData GetContext(TextractClaimImage zonedTextractImage, PointF coordinate, TextractElements selectedElements)
        {

            Field element = null;
            string elementType = null;
            string jsonData = null;
            IEnumerable<Cell> cellElements = null;

            element = zonedTextractImage.Page?.Form.GetFieldByFieldCoordinate(coordinate);

            //if (Cache.ContainsKey(element.Match))
            //    return Cache[element.Match];

            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.ContractResolver = new DynamicContractResolver(new string[] { "Polygon" });

            if (selectedElements.HasFlag(TextractElements.Field) | selectedElements.HasFlag(TextractElements.FieldKey) | selectedElements.HasFlag(TextractElements.FieldValue))
            {
                elementType = TextractElements.Field.ToString();
                //element = zonedTextractImage.Page?.Form.GetFieldByFieldCoordinate(coordinate);
                jsonData = JsonConvert.SerializeObject(element, Formatting.Indented, settings);
            }

            if ((jsonData == "[]" | jsonData == "null") && selectedElements.HasFlag(TextractElements.Table))
            {
                elementType = TextractElements.Table.ToString();
                cellElements = zonedTextractImage.Page?.Tables[0].GetCellsByCoordinate(coordinate);
                jsonData = JsonConvert.SerializeObject(cellElements, Formatting.Indented, settings);
            }

            if (element == null & cellElements?.Count() == 0)
                return default(ContextData);

            string mappedPropertyAttributeInstanceName = element?.Match;
            string mappedPropertyGroupName = element?.Key.GroupName;

            PropertyInfo mappedGroupPropertyInfo = OCR.Mapper
                        .FindPropertyByMappedAttributeName(typeof(ADA.ADAClaim), mappedPropertyGroupName);

            PropertyInfo mappedPropertyInfo = OCR.Mapper
                        .FindPropertyByMappedAttributeName(typeof(ADA.ADAClaim), mappedPropertyAttributeInstanceName);

            //bool isRequired = mappedGroupPropertyInfo.CustomAttribute<IMappedPropertyAttribute>().Required
            //    | mappedPropertyInfo.CustomAttribute<IMappedPropertyAttribute>().Required;

            //IEnumerable<PropertyInfo> comositeValueProperties =
            //    OCR.Mapper.FindPropertiesByCompositeAttributeName(typeof(ADA.ADAClaim), mappedPropertyName);

            //IEnumerable<PropertyInfo> groupFieldProperties =
            //OCR.Mapper.FindPropertiesByGroupFieldAttributeName(typeof(ADA.ADAClaim), mappedPropertyGroupName);

            //var mappedPropertAttribute = mappedPropertyInfo?.CustomAttributes
            //        .Where(a =>
            //            a.AttributeType != typeof(System.Xml.Serialization.XmlIgnoreAttribute)
            //            ).SingleOrDefault();

            //var mappedPropertAttributeType = mappedPropertAttribute?.AttributeType;
            // ToDo: Cache all the PropertyInfo values so we don't keep running this reflection
            var compositeValuePropertyItems = OCR.Mapper.GetCompositeValuePropertyItems(typeof(ADA.ADAClaim))
                                                .Where(i => i.Attribute.Name == mappedPropertyAttributeInstanceName);

            var mappedGroupPropertyCustAttr = mappedGroupPropertyInfo?.CustomAttribute<IMappedPropertyAttribute>();
            var mappedPropertyCustAttr = mappedPropertyInfo?.CustomAttribute<IMappedPropertyAttribute>();

            var mappedPropertAttribute = mappedPropertyCustAttr?.GetType();

            bool isRequired = (mappedPropertyCustAttr?.Required
                                | mappedGroupPropertyCustAttr?.Required
                                | compositeValuePropertyItems.Any(c => c.IsRequired)
                                ) ?? false;


            //isRequired = isRequired | compositeValuePropertyItems.Any(c => c.IsRequired);

            //var groupFieldPropertyItems = OCR.Mapper.GetCompositeValuePropertyItems(typeof(ADA.ADAClaim))
            //                                    .Where(i => i.Attribute.GroupName == mappedPropertyGroupName);

            var contextData = new ContextData
            {
                Coordinate = coordinate,
                CellElements = cellElements,
                ElementType = elementType,
                JSONdata = jsonData,
                SelectedElements = selectedElements,
                Element = element,
                IsFieldRequired = isRequired,
                MappedGroupPropertyInfo = mappedGroupPropertyInfo,
                MappedPropertyGroupName = mappedPropertyGroupName,
                MappedPropertyInfo = mappedPropertyInfo,
                MappedPropertyAttributeInstanceName = mappedPropertyAttributeInstanceName,
                MappedPropertAttribute = mappedPropertAttribute,
                CompositeValuePropertyItems = compositeValuePropertyItems
                //GroupFieldPropertyItems = groupFieldPropertyItems
            };

            //Cache.Add(contextData.Element.Match, contextData);

            return contextData;
        }
    }
}
