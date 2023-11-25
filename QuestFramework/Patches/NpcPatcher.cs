using HarmonyLib;
using QuestFramework.Core.Events;
using QuestFramework.Core.Patching;
using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuestFramework.Patches
{
    internal class NpcPatcher : Patcher
    {
        public override void Apply(Harmony harmony, IMonitor monitor)
        {
            harmony.Patch(
                original: RequireMethod<NPC>(nameof(NPC.checkAction)),
                prefix: GetHarmonyMethod(nameof(Before_checkAction))
            );
        }

        private static bool Before_checkAction(Farmer who, GameLocation l, NPC __instance, ref bool __result)
        {
            if (__instance.IsInvisible || !who.CanMove)
            {
                return true;
            }

            __result = QuestCoreMod.Events.OnInteract(who, __instance, l);

            return !__result;
        }
    }
}
