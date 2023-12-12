using QuestFramework.Core;

namespace QuestFramework.Offering
{
    internal record NpcQuestOffer
    {
        public string QuestId { get; init; }
        public QuestMark IndicatorType { get; init; }
        public string Requester { get; init; }
        public string DialogueKey { get; init; }

        public NpcQuestOffer(string questId, QuestMark indicatorType, string requester, string? dialogueKey = null)
        {
            QuestId = questId;
            IndicatorType = indicatorType;
            Requester = requester;
            DialogueKey = dialogueKey ?? "quest_" + questId;
        }
    }
}
