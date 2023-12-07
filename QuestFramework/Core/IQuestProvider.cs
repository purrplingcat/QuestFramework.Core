using StardewModdingAPI;
using StardewValley;
namespace QuestFramework.Core
{
    public interface IQuestProvider
    {
        bool CanStartQuestNow(QuestMetadata questMetadata, Farmer player);
        ICustomQuest? CreateQuest(QuestMetadata metadata);
    }
}
