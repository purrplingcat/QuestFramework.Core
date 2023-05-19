using HarmonyLib;
using StardewModdingAPI;

namespace QuestFramework.Framework.Patching
{
    internal static class HarmonyPatcher
    {
        public static Harmony Apply(Mod mod, Patcher[] patchers)
        {
            var harmony = new Harmony(mod.ModManifest.UniqueID);

            foreach (var patcher in patchers)
            {
                try
                {
                    patcher.Apply(harmony, mod.Monitor);
                }
                catch (Exception ex)
                {
                    mod.Monitor.Log($"Failed to apply '{patcher.GetType().FullName}' patcher; some features may not work correctly. Technical details:\n{ex}", LogLevel.Error);
                }
            }

            return harmony;
        }
    }
}
