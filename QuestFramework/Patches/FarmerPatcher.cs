﻿using HarmonyLib;
using QuestFramework.Extensions;
using QuestFramework.Core;
using QuestFramework.Core.Patching;
using StardewModdingAPI;
using StardewValley;

namespace QuestFramework.Patches
{
    internal class FarmerPatcher : Patcher
    {
        public override void Apply(Harmony harmony, IMonitor monitor)
        {
            harmony.Patch(
                original: RequireMethod<Farmer>(nameof(Farmer.hasQuest)),
                prefix: GetHarmonyMethod(nameof(Before_hasQuest))
            );
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

        private static bool Before_hasQuest(Farmer __instance, string id)
        {
            if (Utils.IsQfQuestId(id))
            {
                __instance.GetQuestManager()?.HasQuest(id);
                return false;
            }

            return true;
        }

        private static bool Before_addQuest(Farmer __instance, string questId)
        {
            if (Utils.IsQfQuestId(questId))
            {
                __instance.GetQuestManager()?.AddQuest(questId);
                return false;
            }

            return true;
        }

        private static bool Before_removeQuest(Farmer __instance, string questID)
        {
            if (Utils.IsQfQuestId(questID))
            {
                __instance.GetQuestManager()?.RemoveQuest(questID);
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
