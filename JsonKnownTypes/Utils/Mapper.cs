namespace QuestFramework.Json.Utils
{
    internal static class Mapper
    {
        internal static JsonDiscriminatorSettings Map(JsonDiscriminatorAttribute entity)
        {
            var settings = new JsonDiscriminatorSettings();

            settings.DiscriminatorFieldName = entity.Name ?? settings.DiscriminatorFieldName;

            return settings;
        }
    }
}
