using Newtonsoft.Json;
using StardewValley.Mods;

namespace QuestFramework.Core.Converters
{
    internal class ModDataConverter : JsonConverter<ModDataDictionary>
    {
        public override ModDataDictionary? ReadJson(JsonReader reader, Type objectType, ModDataDictionary? existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var target = new ModDataDictionary();
            var source = serializer.Deserialize<Dictionary<string, string>>(reader) 
                ?? new Dictionary<string, string>();

            foreach (var key in source.Keys)
            {
                target[key] = source[key];
            }

            return target;
        }

        public override void WriteJson(JsonWriter writer, ModDataDictionary? value, JsonSerializer serializer)
        {
            var target = new Dictionary<string, string>();

            if (value != null)
            {
                foreach (var key in value.Keys)
                {
                    target[key] = value[key];
                }
            }

            serializer.Serialize(writer, target);
        }
    }
}
