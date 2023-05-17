using QuestFramework.API;

namespace QuestFramework.Quests.Providers
{
    public class DefaultQuestProvider : IQuestProvider
    {
        public ICustomQuest? CreateQuest(IQuestMetadata metadata)
        {
            return new StandardQuest(metadata.QualifiedId, metadata.LocalId, metadata.TypeIdentifier);
        }
    }
}
