using QuestFramework.API;
using QuestFramework.API.Attributes;
using System.Reflection;

namespace QuestFramework.Framework.Converters
{
    internal class CustomQuestConverter : PolymorphicConverter<ICustomQuest>
    {
        private static readonly Dictionary<string, Type> _knownTypes = new();

        public override ICustomQuest? CreateInstance(string typeName)
        {
            if (_knownTypes.TryGetValue(typeName, out Type? type))
            {
                return (ICustomQuest?)Activator.CreateInstance(type);
            }

            return null;
        }

        public override string? ResolveDiscriminator(ICustomQuest polymorph)
        {
            var type = polymorph.GetType();

            return _knownTypes.FirstOrDefault(kv => kv.Value == type).Key;
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
