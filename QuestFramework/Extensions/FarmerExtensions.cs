using QuestFramework.Core;
using StardewValley;

namespace QuestFramework.Extensions
{
    public static class FarmerExtensions
    {
        public static IQuestManager? GetQuestManager(this Farmer farmer)
        {
            if (QuestManager.Managers.TryGetValue(farmer.UniqueMultiplayerID, out var manager))
            {
                return manager;
            }

            return null;
        }

        public static void AddQuest(this Farmer farmer, ICustomQuest quest)
        {
            farmer.GetQuestManager()?.AddQuest(quest);
        }
    }
}
