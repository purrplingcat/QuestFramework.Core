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

        internal static void AddJsonTypes(this DiscriminatorValues discriminatorValues, JsonTypeDefinition[] typeDefs)
        {
            foreach (var typeDef in typeDefs)
            {
                discriminatorValues.AddType(typeDef.Type, typeDef.Discriminator);
            }
        }

        internal static void AddAutoDiscriminators(this DiscriminatorValues discriminatorValues, JsonTypeDefinition[] typeDefs)
        {
            foreach (var typeDef in typeDefs)
            {
                if (discriminatorValues.Contains(typeDef.Type))
                    continue;

                discriminatorValues.AddType(typeDef.Type, typeDef.Discriminator);
            }
        }
    }
}
