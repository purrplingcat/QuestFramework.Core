using System;

namespace QuestFramework.Json
{
    /// <summary>
    /// Add discriminator to specified type
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = true)]
    public class JsonTypeIncludeAttribute : Attribute
    {
        public Type Type { get; }

        public string Discriminator { get; }

        public JsonTypeIncludeAttribute(Type type)
        {
            Type = type;
        }

        public JsonTypeIncludeAttribute(Type type, string discriminator)
        {
            Type = type;
            Discriminator = discriminator;
        }
    }
}
