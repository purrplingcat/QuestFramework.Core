using System;

namespace QuestFramework.Json
{
    /// <summary>
    /// Add discriminator to type which is used with
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, Inherited = false)]
    public class JsonTypeAttribute : Attribute
    {
        public string Discriminator { get; }

        public JsonTypeAttribute()
        { }

        public JsonTypeAttribute(string discriminator)
        {
            Discriminator = discriminator;
        }
    }
}
