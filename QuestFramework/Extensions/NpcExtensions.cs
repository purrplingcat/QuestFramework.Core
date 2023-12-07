using QuestFramework.Core;
using QuestFramework.Internal;
using StardewValley;
using StardewValley.Quests;

namespace QuestFramework.Extensions
{
    public static class NpcExtensions
    {
        public static QuestIndicator GetQuestIndicator(this NPC npc)
        {
            return QuestCoreMod.IndicatorManager.GetIndicator(npc.Name);
        }

        public static void SetQuestIndicator(this NPC npc, QuestIndicator indicator)
        {
            QuestCoreMod.IndicatorManager.Indicators[npc.Name] = indicator;
        }

        public static void SetMarker(this NPC npc, string key, QuestMark mark = QuestMark.Default)
        {
            npc.GetQuestIndicator().Set(key, mark);
        }

        public static void ClearMarker(this NPC npc, string key)
        {
            npc.GetQuestIndicator().Clear(key);
        }

        public static void ClearAllMarkers(this NPC npc)
        {
            npc.GetQuestIndicator().Clear();
        }

        public static void OfferQuest(this NPC npc, Farmer farmer, string questId, string? dialogueKey = null)
        {
            if (farmer is null)
            {
                throw new ArgumentNullException(nameof(farmer));
            }

            if (questId is null)
            {
                throw new ArgumentNullException(nameof(questId));
            }
            
            var offer = npc.TryGetDialogue(dialogueKey ?? "quest_" + questId) 
                ?? new Dialogue(npc, "quest_" + questId, dialogueKey);

            offer.onFinish += () => farmer.addQuest(questId);
            npc.CurrentDialogue.Push(offer);
            Game1.drawDialogue(npc);
        }

        public static void OfferSpecialOrder(this NPC npc, Farmer farmer, string orderId, string? dialogueKey)
        {
            if (orderId is null)
            {
                throw new ArgumentNullException(nameof(orderId));
            }

            var offer = Dialogue.TryGetDialogue(npc, dialogueKey ?? "order_" + orderId)
                ?? new Dialogue(npc, "order_" + orderId, dialogueKey);

            offer.onFinish += () => farmer.team.AddSpecialOrder(orderId);
            npc.CurrentDialogue.Push(offer);
            Game1.drawDialogue(npc);
        }
    }
}
