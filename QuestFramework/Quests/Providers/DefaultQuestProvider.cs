using QuestFramework.API;
using QuestFramework.Quests.Objectives;

namespace QuestFramework.Quests.Providers
{
    public class DefaultQuestProvider : IQuestProvider
    {
        public ICustomQuest? CreateQuest(IQuestMetadata metadata)
        {
            var quest = new StandardQuest(metadata.QualifiedId, metadata.LocalId, metadata.TypeIdentifier)
            {
                Name = "[Test]",
                Description = "[Description]",
            };

            var co = new CustomObjective
            {
                Description = "Test objective [Season]"
            };
            co.ModData["test"] = "testing";
            quest.Objectives.Add(co);
            quest.ShowNew = true;
            return quest;
        }
    }
}
