using StardewValley;

namespace QuestFramework.Offering
{
    public interface INpcQuestOfferManager
    {
        void AddQuestOffer(string npcName, NpcQuestOffer questOffer);
        void ClearQuestOffers();
        void ClearQuestOffers(string npcName);
        void OfferQuestToPlayer(NPC npc, Farmer player);
    }
}
