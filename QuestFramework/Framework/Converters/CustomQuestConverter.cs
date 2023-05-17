using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using QuestFramework.API;
using QuestFramework.API.Attributes;
using System.Reflection;

namespace QuestFramework.Framework.Converters
{
    internal class CustomQuestConverter : JsonConverter<ICustomQuest>
    {
        private const string TYPE_KEY = "@type";

        private static readonly Dictionary<string, Type> _knownTypes = new();

        public override ICustomQuest? ReadJson(JsonReader reader, Type objectType, ICustomQuest? existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var raw = serializer.Deserialize<JObject>(reader);
            string? typeName = (string?)raw?[TYPE_KEY];

            if (typeName != null && _knownTypes.TryGetValue(typeName, out Type? type)) 
            {
                ICustomQuest? quest = (ICustomQuest?)Activator.CreateInstance(type);

                if (raw != null && quest != null)
                {
                    serializer.Populate(raw.CreateReader(), quest);

                    return quest;
                }
            }

            return null;
        }

        public override void WriteJson(JsonWriter writer, ICustomQuest? value, JsonSerializer serializer)
        {
            if (value == null) { return; }

            var raw = JObject.FromObject(value, serializer);

            raw[TYPE_KEY] = GetTypeName(value.GetType());
            serializer.Serialize(writer, raw);
        }

        private static string GetTypeName(Type type)
        {
            var typeName = _knownTypes.FirstOrDefault(kv => kv.Value == type).Key;

            if (typeName == null)
            {
                throw new JsonSerializationException($"Unrecognized type for serialization: {type.FullName}");
            }

            return typeName;
        }

        public static void RegisterType(Type type, string? name = null)
        {
            var ifaceType = typeof(ICustomQuest);
            name ??= type.GetCustomAttribute<CustomQuestAttribute>()?.Name ?? type.Name;

            if (!ifaceType.IsAssignableFrom(type))
            {
                throw new Exception($"Unable to register quest type '{name}': Type {type.FullName} doesn't implement {ifaceType.FullName}");
            }

            _knownTypes[name] = type;
            Logger.Debug($"Registered custom quest type '{name}' ({type.FullName})");
        }
    }
}
