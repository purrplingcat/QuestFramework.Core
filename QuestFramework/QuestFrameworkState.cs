using StardewModdingAPI;

namespace QuestFramework
{
    public class QuestFrameworkState
    {
        public ISemanticVersion? Version { get; set; }
        public Dictionary<long, QuestManagerState> Quests { get; set; } = new();
    }
}
