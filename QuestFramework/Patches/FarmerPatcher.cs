using HarmonyLib;
using QuestFramework.Extensions;
using QuestFramework.Framework;
using QuestFramework.Framework.Patching;
using StardewModdingAPI;
using StardewValley;

namespace QuestFramework.Patches
{
    internal class FarmerPatcher : Patcher
    {
        public override void Apply(Harmony harmony, IMonitor monitor)
        {
            harmony.Patch(
                original: RequireMethod<Farmer>(nameof(Farmer.addQuest)),
                prefix: GetHarmonyMethod(nameof(Before_addQuest))
            );

            harmony.Patch(
                original: RequireMethod<Farmer>(nameof(Farmer.removeQuest)),
                prefix: GetHarmonyMethod(nameof(Before_removeQuest))
            );
        }

        private static bool Before_addQuest(Farmer __instance, string questId)
        {
            if (Utils.IsQfQuestId(questId, out string qfQuestId))
            {
                __instance.GetQuestManager().AddQuest(qfQuestId);
                return false;
            }

            return true;
        }

        private static bool Before_removeQuest(Farmer __instance, string questID)
        {
            if (Utils.IsQfQuestId(questID, out string qfQuestId))
            {
                __instance.GetQuestManager().RemoveQuest(qfQuestId);
                return false;
            }

            return true;
        }
    }
}
