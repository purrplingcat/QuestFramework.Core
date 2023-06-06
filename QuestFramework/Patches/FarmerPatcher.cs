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
            harmony.Patch(
                original: RequireMethod<Farmer>(nameof(Farmer.hasNewQuestActivity)),
                postfix: GetHarmonyMethod(nameof(After_hasNewQuestActivity))
            );
            harmony.Patch(
                original: RequirePropertyGetter<Farmer>(nameof(Farmer.hasVisibleQuests)),
                postfix: GetHarmonyMethod(nameof(After_get_hasVisibleQuests))
            );
        }

        private static bool Before_addQuest(Farmer __instance, string questId)
        {
            if (Utils.IsQfQuestId(questId, out string qfQuestId))
            {
                __instance.GetQuestManager()?.AddQuest(qfQuestId);
                return false;
            }

            return true;
        }

        private static bool Before_removeQuest(Farmer __instance, string questID)
        {
            if (Utils.IsQfQuestId(questID, out string qfQuestId))
            {
                __instance.GetQuestManager()?.RemoveQuest(qfQuestId);
                return false;
            }

            return true;
        }

        private static void After_hasNewQuestActivity(ref bool __result)
        {
            var manager = Game1.player.GetQuestManager();

            if (manager == null) { return; }

            foreach (var q in manager.Quests)
            {
                if (q == null) { continue; }

                if (!q.IsHidden() && (q.ShouldDisplayAsNew() || q.ShouldDisplayAsComplete()))
                {
                    __result = true;
                    return;
                }
            }
        }

        private static void After_get_hasVisibleQuests(ref bool __result)
        {
            var manager = Game1.player.GetQuestManager();

            if (manager == null)
                return;

            foreach (var quest in manager.Quests)
            {
                if (quest != null && !quest.IsHidden())
                {
                    __result = true;
                    return;
                }
            }
        }
    }
}
