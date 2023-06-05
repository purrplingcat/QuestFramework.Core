using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;

namespace QuestFramework.Game.Menus
{
    internal class CustomQuestLog : QuestLog
    {
        public static void HookOnMenu(IDisplayEvents events)
        {
            events.MenuChanged += (_, e) =>
            {
                if (!Context.IsWorldReady) { return; }
                if (e.NewMenu is QuestLog and not CustomQuestLog)
                {
                    Game1.activeClickableMenu = new CustomQuestLog();
                }
            };
        }
    }
}
