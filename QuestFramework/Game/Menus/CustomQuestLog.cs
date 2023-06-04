using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuestFramework.Game.Menus
{
    internal class CustomQuestLog : QuestLog
    {
        public static void OnMenuChanged(object? sender, MenuChangedEventArgs e)
        {
            if (!Context.IsWorldReady) { return; }
            if (e.NewMenu is QuestLog and not CustomQuestLog)
            {
                Game1.activeClickableMenu = new CustomQuestLog();
            }
        }
    }
}
