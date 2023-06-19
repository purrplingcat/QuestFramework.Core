using Newtonsoft.Json;
using QuestFramework.API;
using QuestFramework.Framework.Converters;

namespace QuestFramework.Framework.Model
{
    internal class QuestManagerState
    {
        [JsonProperty("quests", ItemConverterType = typeof(QuestConverter<ICustomQuest>))]
        public List<ICustomQuest> Quests { get; set; } = new();

        [JsonProperty("applied_rules")]
        public List<string> Rules { get; set; } = new();
    }
}
