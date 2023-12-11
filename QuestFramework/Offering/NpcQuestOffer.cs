using QuestFramework.Core;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
