using QuestFramework.Extensions;
using StardewValley;
using StardewValley.Quests;
using StardewValley.SpecialOrders;

namespace QuestFramework.Core
{
    public class QuestDialogue : Dialogue
    {
        public bool IsSpecialOrder { get; set; }
        public string? QuestId { get; set; }
        public bool ShowIcon { get; set; }

        public QuestDialogue(NPC speaker, string translationKey, string dialogueText) : base(speaker, translationKey, dialogueText)
        {
        }

        public override string exitCurrentDialogue()
        {
            if (isOnFinalDialogue() && QuestId != null)
            {
                AcceptAttachedQuest();
            }

            return base.exitCurrentDialogue();
        }

        protected virtual void AcceptAttachedQuest()
        {
            if (IsSpecialOrder)
            {
                farmer.team.AddSpecialOrder(QuestId);
            }
            else
            {
                farmer.addQuest(QuestId);
            }
        }
    }
}
