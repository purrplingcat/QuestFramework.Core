using Newtonsoft.Json;
using QuestFramework.API;
using QuestFramework.Framework.Converters;

namespace QuestFramework.Model
{
    internal class QuestManagerState
    {
        [JsonProperty(ItemConverterType = typeof(CustomQuestConverter))]
        public List<ICustomQuest>? Quests { get; set; }
    }
}
