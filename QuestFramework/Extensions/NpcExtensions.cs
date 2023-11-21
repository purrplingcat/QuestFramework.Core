using QuestFramework.Core;
using StardewValley;

namespace QuestFramework.Extensions
{
    public static class NpcExtensions
    {
        public static QuestIndicator GetQuestIndicator(this NPC npc)
        {
            var indicatorManager = QuestCoreMod.IndicatorManager;

            if (!indicatorManager.Indicators.TryGetValue(npc.Name, out var indicator))
            {
                indicator = new QuestIndicator();
                indicatorManager.Indicators[npc.Name] = indicator;
            }

            return indicator;
        }

        public static void SetQuestIndicator(this NPC npc, QuestIndicator indicator)
        {
            QuestCoreMod.IndicatorManager.Indicators[npc.Name] = indicator;
        }

        public static void SetMark(this NPC npc, string key, QuestMark mark = QuestMark.Default)
        {
            npc.GetQuestIndicator().Set(key, mark);
        }

        public static void ClearMark(this NPC npc, string key)
        {
            npc.GetQuestIndicator().Clear(key);
        }

        public static void ClearAllMarks(this NPC npc)
        {
            npc.GetQuestIndicator().Clear();
        }
    }
}
