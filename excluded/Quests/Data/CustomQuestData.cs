using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuestFramework.Quests.Data
{
    public class CustomQuestData
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Type { get; set; } = "Standard";
        public int Duration { get; set; }
        public string? Condition { get; set; }
        public Dictionary<string, QuestObjectiveData> Objectives { get; set; } = new();
    }
}
