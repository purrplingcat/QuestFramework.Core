using QuestFramework.Extensions;
using QuestFramework.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Quests;

namespace QuestFramework.Game.Menus
{
    internal class CustomQuestLog : QuestLog
    {
        protected override IList<IQuest> GetAllQuests()
        {
            var quests = new List<IQuest>();
            var manager = Game1.player.GetQuestManager();

            if (manager != null)
            {
                // Add QF quests
                for (int i = manager.Quests.Count - 1; i >= 0; i--)
                {
                    var quest = manager.Quests[i];
                    if (quest != null && !quest.IsHidden())
                    {
                        quests.Add(quest);
                    }
                }
            }

            // Forward vanilla quests
            quests.AddRange(base.GetAllQuests());

            return quests;
        }

        public static void HookOnMenu(IDisplayEvents events)
        {
            events.MenuChanged += (_, e) =>
            {
                if (!Context.IsWorldReady) { return; }
                if (e.NewMenu is QuestLog and not CustomQuestLog)
                {
                    Game1.activeClickableMenu = new CustomQuestLog();
                    Logger.Trace("QuestLog menu is overriden by CustomQuestLog menu");
                }
            };
        }
    }
}
