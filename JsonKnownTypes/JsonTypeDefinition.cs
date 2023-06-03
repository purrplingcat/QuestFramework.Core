using System;

namespace JsonKnownTypes
{
    internal class JsonTypeDefinition
    {
        public string Discriminator { get; }
        public Type Type { get; }

        public JsonTypeDefinition(Type type, string discriminator = null)
        {
            Type = type;
            Discriminator = discriminator ?? type.Name;
        }
    }
}