using Newtonsoft.Json;
using QuestFramework.API;
using QuestFramework.Framework.Converters;

namespace QuestFramework.Framework.Model
{
    internal class QuestManagerState
    {
        [JsonProperty(ItemConverterType = typeof(QuestConverter<ICustomQuest>))]
        public List<ICustomQuest> Quests { get; set; } = new();
    }
}
