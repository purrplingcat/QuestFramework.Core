using QuestFramework.Core;
using QuestFramework.Core.Events;
using QuestFramework.Extensions;
using StardewModdingAPI.Utilities;
using StardewValley;

namespace QuestFramework.Offering
{
    internal class NpcQuestManager
    {
        private readonly PerScreen<Dictionary<string, Stack<NpcQuestOffer>>> _questOffers;
        private readonly QuestIndicatorManager indicatorManager;

        protected Dictionary<string, Stack<NpcQuestOffer>> QuestOffers => _questOffers.Value;

        internal NpcQuestManager(QuestIndicatorManager indicatorManager)
        {
            _questOffers = new(() => new());
            this.indicatorManager = indicatorManager;
        }

        public void AddQuestOffer(string npcName, NpcQuestOffer questOffer)
        {
            Stack<NpcQuestOffer> offers = GetOffersFor(npcName);

            offers.Push(questOffer);
            UpdateQuestMarker(npcName, offers);
        }

        protected Stack<NpcQuestOffer> GetOffersFor(string npcName)
        {
            if (!QuestOffers.TryGetValue(npcName, out var offers))
            {
                offers = new Stack<NpcQuestOffer>();
                QuestOffers[npcName] = offers;
            }

            return offers;
        }

        private void UpdateQuestMarker(string npcName, Stack<NpcQuestOffer> stack)
        {
            indicatorManager.GetIndicator(npcName)
                .Set("quest_offer",
                    stack.Count > 0 ? stack.Peek().IndicatorType : QuestMark.None
                );
        }

        public void ClearQuestOffers()
        {
            foreach (string npcName in QuestOffers.Keys)
            {
                ClearQuestOffers(npcName);
            }
        }

        public void ClearQuestOffers(string npcName)
        {
            var offers = GetOffersFor(npcName);
            offers.Clear();
            UpdateQuestMarker(npcName, offers);
        }

        public void OfferQuestToPlayer(NPC npc, Farmer player)
        {
            Stack<NpcQuestOffer> offers = GetOffersFor(npc.Name);

            if (offers.Count > 0)
            {
                var offer = offers.Pop();
                npc.OfferQuest(player, offer.QuestId, offer.DialogueKey);
            }

            UpdateQuestMarker(npc.Name, offers);
        }

        internal void OnInteract(object sender, InteractEventArgs e)
        {
            var offers = GetOffersFor(e.NPC.Name);

            if (offers.Count > 0 && !e.IsSupressed)
            {
                e.Supress();
                OfferQuestToPlayer(e.NPC, e.Farmer);
            }

            UpdateQuestMarker(e.NPC.Name, offers);
        }

        internal void Reset()
        {
            _questOffers.ResetAllScreens();
        }
    }
}
