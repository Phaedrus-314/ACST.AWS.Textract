
namespace ACST.AWS.Common
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;

    public class IgnoreJsonAttributesResolver : DefaultContractResolver
    {
        protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
        {
            IList<JsonProperty> props = base.CreateProperties(type, memberSerialization);
            foreach (var prop in props)
            {
                prop.Ignored = false;   // Ignore [JsonIgnore]
                prop.Converter = null;  // Ignore [JsonConverter]
                prop.PropertyName = prop.UnderlyingName;  // restore original property name
            }
            return props;
        }

    }

    public class DynamicContractResolver : DefaultContractResolver
    {
        private readonly string[] PropertiesToSuppress;

        public DynamicContractResolver(string[] propertiesToSuppress)
        {
            this.PropertiesToSuppress = propertiesToSuppress;
        }

        protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
        {
            IList<JsonProperty> properties = base.CreateProperties(type, memberSerialization);

            // only serializer properties that start with the specified character
            //properties =
            //    properties.Where(p => p.PropertyName.StartsWith(_startingWithChar.ToString())).ToList();

            properties =
                properties.Where(p => !this.PropertiesToSuppress.Contains(p.PropertyName)).ToList();

            return properties;
        }
    }
}
