using QuestFramework.API;
using QuestFramework.Framework;
using StardewValley;

namespace QuestFramework.Extensions
{
    public static class FarmerExtensions
    {
        public static IQuestManager GetQuestManager(this Farmer farmer)
        {
            return QuestManager.Managers[farmer.UniqueMultiplayerID];
        }
    }
}
