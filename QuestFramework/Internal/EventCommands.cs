using QuestFramework.Extensions;
using StardewValley;

namespace QuestFramework.Internal
{
    internal static class EventCommands
    {
        public static void AddQuest(Event @event, string[] args, EventContext context)
        {
            if (!ArgUtility.TryGet(args, 1, out var questId, out var error))
            {
                context.LogErrorAndSkip(error);
                return;
            }

            Game1.player.GetQuestManager()?.AddQuest(questId);
            @event.CurrentCommand++;
        }

        internal static void RegisterCommands(string prefix)
        {
            string Prefix(string name) => $"{prefix}_{name}";

            Event.RegisterCustomCommand(Prefix("AddQuest"), AddQuest);
        }
    }
}
