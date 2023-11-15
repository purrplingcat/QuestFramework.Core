using StardewValley.Quests;

namespace QuestFramework.API
{
    public interface IQuestMenu
    {
        IQuest? GetCurrentQuest();
        IList<IQuestObjective> GetCurrentObjectives();
    }
}
