using System;

namespace JsonKnownTypes
{
    /// <summary>
    /// Adds fallback type when type lookup fails.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface)]
    public class JsonTypeFallbackAttribute : Attribute
    {
        public Type FallbackType { get; }

        public JsonTypeFallbackAttribute(Type fallbackType)
        {
            FallbackType = fallbackType ?? throw new ArgumentNullException(nameof(fallbackType));
        }
    }
}
