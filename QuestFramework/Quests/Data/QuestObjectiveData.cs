namespace QuestFramework.Quests.Data
{
    public class QuestObjectiveData
    {
        public string Type { get; set; } = "";
        public string Description { get; set; } = "";
        public string? Condition { get; set; }
        public int RequiredCount { get; set; }
        public Dictionary<string, string> Data { get; set; } = new();
    }
}
