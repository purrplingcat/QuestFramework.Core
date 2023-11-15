using System;

namespace QuestFramework.Json
{
    /// <summary>
    /// Manage discriminator settings
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface)]
    public class JsonDiscriminatorAttribute : Attribute
    {
        /// <summary>
        /// Discriminator field name in json representation
        /// </summary>
        public string Name { get; set; }
    }
}
