using StardewValley;
namespace QuestFramework.API
{
    public interface IQuestProvider
    {
        ICustomQuest? CreateQuest(IQuestMetadata metadata);
    }
}
