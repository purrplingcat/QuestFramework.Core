using QuestFramework.Core;
using StardewModdingAPI;
using StardewValley;
namespace QuestFramework.API
{
    public interface IQuestProvider
    {
        ICustomQuest? CreateQuest(QuestMetadata metadata);
    }
}
