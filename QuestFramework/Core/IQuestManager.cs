using StardewValley;

namespace QuestFramework.Core
{
    public interface IQuestManager
    {
        long PlayerId { get; }
        Farmer Player { get; }
        IList<ICustomQuest> Quests { get; }
        bool IsActive { get; }

        public bool CanStartQuestNow(string questId);
        void AddQuest(string questId, int? seed = null, bool ignoreDuplicities = false);
        void AddQuest(ICustomQuest quest, bool ignoreDuplicities = false);
        bool HasQuest(string questId);
        void RemoveQuest(string questId);
        void Update();
    }
}
