using StardewValley.Quests;

namespace QuestFramework.Menus
{
    public class QuestRendererSelector<TQuest> : IRendererSelector where TQuest : IQuest
    {
        public int Priority { get; }

        public QuestRendererSelector(int priority = 0)
        {
            Priority = priority;
        }

        public bool ShouldUseRenderer(IQuest quest)
        {
            return quest is TQuest;
        }
    }
}
