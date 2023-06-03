using System;

namespace JsonKnownTypes
{
    internal static class AttributesManager
    {
        internal static JsonTypeIncludeAttribute[] GetJsonTypeInclude(Type type) =>
            (JsonTypeIncludeAttribute[])Attribute.GetCustomAttributes(type, typeof(JsonTypeIncludeAttribute));

        internal static JsonTypeAttribute GetJsonTypeAttribute(Type type) =>
            (JsonTypeAttribute)Attribute.GetCustomAttribute(type, typeof(JsonTypeAttribute));

        internal static JsonDiscriminatorAttribute GetJsonDiscriminatorAttribute(Type type) =>
            (JsonDiscriminatorAttribute)Attribute.GetCustomAttribute(type, typeof(JsonDiscriminatorAttribute));
        
        internal static JsonTypeFallbackAttribute GetJsonTypeFallbackAttribute(Type type) =>
            (JsonTypeFallbackAttribute)Attribute.GetCustomAttribute(type, typeof(JsonTypeFallbackAttribute));
    }
}
