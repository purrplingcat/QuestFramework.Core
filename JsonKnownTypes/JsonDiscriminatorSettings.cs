namespace JsonKnownTypes
{
    public class JsonDiscriminatorSettings
    {
        /// <summary>
        /// Discriminator field name in json representation
        /// </summary>
        public string DiscriminatorFieldName { get; set; } = "$type";
    }
}
