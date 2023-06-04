using HarmonyLib;
using QuestFramework.Extensions;
using QuestFramework.Framework.Patching;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Quests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuestFramework.Patches
{
    [Obsolete("Remove with next SDV 1.6 build")]
    internal class QuestLogPatcher : Patcher
    {
        public override void Apply(Harmony harmony, IMonitor monitor)
        {
            harmony.Patch(
                original: RequireMethod<QuestLog>("paginateQuests"),
                postfix: GetHarmonyMethod(nameof(After_paginateQuests))
            );
        }

        private static void After_paginateQuests(ref List<List<IQuest>> ___pages)
        {
            var pages = new List<List<IQuest>>();
            var manager = Game1.player.GetQuestManager();

            for (int i = manager.Quests.Count - 1; i >= 0; i--)
            {
                int which = i;
                while (pages.Count <= which / 6)
                {
                    pages.Add(new List<IQuest>());
                }
                if (!manager.Quests[i].IsHidden())
                {
                    pages[which / 6].Add(manager.Quests[i]);
                }
            }

            ___pages.InsertRange(0, pages);
        }
    }
}
