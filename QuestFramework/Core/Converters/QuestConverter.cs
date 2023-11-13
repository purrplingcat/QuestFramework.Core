using JsonKnownTypes;
using JsonKnownTypes.Exceptions;
using Newtonsoft.Json;

namespace QuestFramework.Core.Converters
{
    internal class QuestConverter<T> : JsonTypeConverter<T>
    {
        public override object? ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            try
            {
                return base.ReadJson(reader, objectType, existingValue, serializer);
            } 
            catch (JsonKnownTypesException ex)
            {
                Logger.Warn(ex.Message);

                return null;
            }
        }
    }
}
