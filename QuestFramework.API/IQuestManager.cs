using StardewValley;

namespace QuestFramework.API
{
    public interface IQuestManager
    {
        long RefId { get; }
        Farmer Player { get; }
        IList<ICustomQuest> Quests { get; }
        bool IsActive { get; }

        void AddQuest(string questId, int? seed = null);
        void AddQuest(ICustomQuest quest);
        void Update();
    }
}