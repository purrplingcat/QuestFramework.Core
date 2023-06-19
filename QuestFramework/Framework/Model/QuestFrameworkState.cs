using Newtonsoft.Json;
using StardewModdingAPI;

namespace QuestFramework.Framework.Model
{
    internal class QuestFrameworkState
    {
        [JsonProperty("version")]
        public ISemanticVersion? Version { get; set; }

        [JsonProperty("managers")]
        public Dictionary<long, QuestManagerState> Managers { get; set; } = new();
    }
}
