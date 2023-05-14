namespace QuestFramework
{
    public class QuestFrameworkState
    {
        public Dictionary<long, QuestManagerState> Quests { get; set; } = new();
    }
}
