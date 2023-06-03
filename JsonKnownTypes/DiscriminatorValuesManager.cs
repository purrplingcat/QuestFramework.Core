using System;

namespace JsonKnownTypes
{
    internal static class DiscriminatorValuesManager
    {
        internal static void AddJsonIncludes<T>(this DiscriminatorValues discriminatorValues)
        {
            var attrs = AttributesManager.GetJsonTypeInclude(typeof(T));
            foreach (var attr in attrs)
            {
                var discriminator = attr.Discriminator ?? attr.Type.Name;
                var typeAttr = AttributesManager.GetJsonTypeAttribute(attr.Type);
                
                if (typeAttr != null && typeAttr.Discriminator != null)
                {
                    discriminator = typeAttr.Discriminator;
                }

                discriminatorValues.AddType(attr.Type, discriminator);
            }

            var fallbackTypeAttribute = AttributesManager.GetJsonTypeFallbackAttribute(typeof(T));
            if (fallbackTypeAttribute != null)
            {
                discriminatorValues.AddFallbackType(fallbackTypeAttribute.FallbackType);
            }
        }

        internal static void AddJsonTypes(this DiscriminatorValues discriminatorValues, Type[] inherited)
        {
            foreach (var type in inherited)
            {
                var attr = AttributesManager.GetJsonTypeAttribute(type);
                if (attr != null && !discriminatorValues.Contains(type))
                {
                    var discriminator = attr.Discriminator ?? type.Name;
                    discriminatorValues.AddType(type, discriminator);
                }   
            }
        }

        internal static void AddAutoDiscriminators(this DiscriminatorValues discriminatorValues, Type[] inherited)
        {
            foreach (var type in inherited)
            {
                if (discriminatorValues.Contains(type))
                    continue;

                discriminatorValues.AddType(type, type.Name);
            }
        }
    }
}
