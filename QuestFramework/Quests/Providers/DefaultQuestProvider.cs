using QuestFramework.API;
using QuestFramework.Quests.Objectives;

namespace QuestFramework.Quests.Providers
{
    public class DefaultQuestProvider : IQuestProvider
    {
        public ICustomQuest? CreateQuest(IQuestMetadata metadata)
        {
            var quest = new StandardQuest(metadata.QualifiedId, metadata.LocalId, metadata.TypeIdentifier);

            var co = new CustomObjective();
            co.modData["test"] = "testing";
            quest.Objectives.Add(co);
            quest.ShowNew = true;
            return quest;
        }
    }
}
