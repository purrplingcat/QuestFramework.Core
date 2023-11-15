using Newtonsoft.Json;
using QuestFramework.Core.Converters;

namespace QuestFramework.Core.Model
{
    internal class QuestManagerState
    {
        [JsonProperty("quests", ItemConverterType = typeof(QuestConverter<ICustomQuest>))]
        public List<ICustomQuest> Quests { get; set; } = new();

        [JsonProperty("applied_rules")]
        public List<string> Rules { get; set; } = new();
    }
}
