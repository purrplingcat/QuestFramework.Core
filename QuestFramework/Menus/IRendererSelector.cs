using QuestFramework.Core;
using StardewValley.Quests;

namespace QuestFramework.Menus
{
    public interface IRendererSelector
    {
        int Priority { get; }

        bool ShouldUseRenderer(IQuest quest);
    }
}
