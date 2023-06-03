using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using JsonKnownTypes.Utils;

namespace JsonKnownTypes
{
    public static class JsonTypesManager
    {
        /// <summary>
        /// Default settings for discriminator
        /// </summary>
        public static JsonDiscriminatorSettings DefaultDiscriminatorSettings { get; set; } =
            new JsonDiscriminatorSettings();

        internal static HashSet<JsonTypeDefinition> KnownTypes { get; } = new();

        public static event Action<Type, string> TypeRegistered;

        internal static DiscriminatorValues GetDiscriminatorValues<T>()
        {
            var discriminatorAttribute = AttributesManager.GetJsonDiscriminatorAttribute(typeof(T));

            var discriminatorSettings = discriminatorAttribute == null 
                ? DefaultDiscriminatorSettings 
                : Mapper.Map(discriminatorAttribute);

            var typeSettings = new DiscriminatorValues(typeof(T), discriminatorSettings.DiscriminatorFieldName);

            typeSettings.AddJsonIncludes<T>();

            var knownTypeDefs = GetFilteredDerived<T>();
            typeSettings.AddAutoDiscriminators(knownTypeDefs);

            return typeSettings;
        }

        private static JsonTypeDefinition[] GetFilteredDerived<T>()
        {
            var type = typeof(T);

            return KnownTypes
                .Where(d => type.IsAssignableFrom(d.Type) && !d.Type.IsInterface && !d.Type.IsAbstract)
                .ToArray();
        }

        public static void RegisterType(params Type[] types)
        {
            foreach (var type in types)
            {
                var typeAttr = AttributesManager.GetJsonTypeAttribute(type);
                var typeDef = new JsonTypeDefinition(type, typeAttr?.Discriminator);

                KnownTypes.Add(typeDef);
                TypeRegistered?.Invoke(typeDef.Type, typeDef.Discriminator);
            }
        }

        public static void RegisterTypesFrom(Assembly assembly, bool all = false)
        {
            foreach (var type in assembly.GetTypes())
            {
                if (all || AttributesManager.GetJsonTypeAttribute(type) != null)
                {
                    RegisterType(type);
                }
            }
        }
    }
}
