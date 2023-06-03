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

        internal static HashSet<Type> KnownTypes { get; } = new HashSet<Type>();

        public static event Action<Type> TypeRegistered;

        internal static DiscriminatorValues GetDiscriminatorValues<T>()
        {
            var discriminatorAttribute = AttributesManager.GetJsonDiscriminatorAttribute(typeof(T));

            var discriminatorSettings = discriminatorAttribute == null 
                ? DefaultDiscriminatorSettings 
                : Mapper.Map(discriminatorAttribute);

            var typeSettings = new DiscriminatorValues(typeof(T), discriminatorSettings.DiscriminatorFieldName);

            typeSettings.AddJsonIncludes<T>();

            var allTypes = GetFilteredDerived<T>();
            typeSettings.AddJsonTypes(allTypes);

            if (discriminatorSettings.UseClassNameAsDiscriminator)
            {
                typeSettings.AddAutoDiscriminators(allTypes);
            }

            return typeSettings;
        }

        private static Type[] GetFilteredDerived<T>()
        {
            var type = typeof(T);

            return KnownTypes
                .Where(x => type.IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract)
                .ToArray();
        }

        public static void RegisterType(params Type[] types)
        {
            foreach (var type in types)
            {
                KnownTypes.Add(type);
                TypeRegistered?.Invoke(type);
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
