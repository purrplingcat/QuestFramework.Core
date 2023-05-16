using QuestFramework.API;
using QuestFramework.Quests;

namespace QuestFramework.Providers
{
    internal class DefaultQuestProvider : IQuestProvider
    {
        public ICustomQuest? CreateQuest(IQuestMetadata metadata)
        {
            return new StandardQuest(metadata.QualifiedId, metadata.LocalId, metadata.TypeIdentifier);
        }
    }
}
