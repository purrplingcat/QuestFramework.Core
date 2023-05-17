using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace QuestFramework.Framework.Converters
{
    public abstract class PolymorphicConverter<T> : JsonConverter<T> where T : class
    {
        private const string DISCRIMINATOR = "@type";

        public abstract T? CreateInstance(string typeName);
        public abstract string? ResolveDiscriminator(T polymorph);

        public override T? ReadJson(JsonReader reader, Type objectType, T? existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var raw = serializer.Deserialize<JObject>(reader);
            string? typeName = (string?)raw?[DISCRIMINATOR];

            if (typeName != null)
            {
                T? polymorph = CreateInstance(typeName);

                if (raw != null && polymorph != null)
                {
                    serializer.Populate(raw.CreateReader(), polymorph);

                    return polymorph;
                }
            }

            return null;
        }

        public override void WriteJson(JsonWriter writer, T? value, JsonSerializer serializer)
        {
            if (value == null) { return; }

            var raw = JObject.FromObject(value, serializer);
            string? discriminator = ResolveDiscriminator(value);

            if (discriminator == null)
            {
                throw new JsonSerializationException($"Unrecognized type for serialization: {value?.GetType().FullName}");
            }

            raw[DISCRIMINATOR] = discriminator;
            serializer.Serialize(writer, raw);
        }
    }
}
