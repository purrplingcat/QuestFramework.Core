using StardewModdingAPI;

namespace QuestFramework.Framework.Model
{
    internal class QuestFrameworkState
    {
        public ISemanticVersion? Version { get; set; }
        public Dictionary<long, QuestManagerState> Managers { get; set; } = new();
    }
}
