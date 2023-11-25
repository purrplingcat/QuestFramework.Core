using StardewValley;
using StardewValley.Monsters;

using QuestFramework.API;
using QuestFramework.Core.Events;

namespace QuestFramework.Core
{
    internal class QuestEvents : IQuestEvents
    {
        public event EventHandler<FishCaughtEventArgs>? FishCaught;
        public event EventHandler<GiftGivenEventArgs>? GiftGiven;
        public event EventHandler<ItemCollectedEventArgs>? ItemCollected;
        public event EventHandler<ItemDeliveredEventArgs>? ItemDelivered;
        public event EventHandler<ItemShippedEventArgs>? ItemShipped;
        public event EventHandler<JKScoreAchievedEventArgs>? JKScoreAchieved;
        public event EventHandler<MineFloorReachedEventArgs>? MineFloorReached;
        public event EventHandler<MonsterSlainEventArgs>? MonsterSlain;
        public event EventHandler<InteractEventArgs>? Interact;

        public void OnFishCaught(Farmer farmer, Item fish)
        {
            FishCaught?.Invoke(this, new FishCaughtEventArgs(farmer, fish));
        }

        public void OnGiftGiven(Farmer farmer, NPC receiver, Item gift)
        {
            GiftGiven?.Invoke(this, new GiftGivenEventArgs(farmer, receiver, gift));
        }

        public void OnItemCollected(Farmer farmer, Item item)
        {
            ItemCollected?.Invoke(this, new ItemCollectedEventArgs(farmer, item));
        }

        public int OnItemDelivered(Farmer farmer, NPC receiver, Item item, bool probe)
        {
            int originalAmount = item.Stack;

            ItemDelivered?.Invoke(this, new ItemDeliveredEventArgs(farmer, receiver, item, probe));

            return originalAmount - item.Stack;
        }

        public void OnItemShipped(Farmer farmer, Item item, int price)
        {
            ItemShipped?.Invoke(this, new ItemShippedEventArgs(farmer, item, price));
        }

        public void OnJKScoreAchieved(Farmer farmer, int score)
        {
            JKScoreAchieved?.Invoke(this, new JKScoreAchievedEventArgs(farmer, score));
        }

        public void OnMineFloorReached(Farmer farmer, int floor)
        {
            MineFloorReached?.Invoke(this, new MineFloorReachedEventArgs(farmer, floor));
        }

        public void OnMonsterSlain(Farmer farmer, Monster monster)
        {
            MonsterSlain?.Invoke(this, new MonsterSlainEventArgs(farmer, monster));
        }

        public bool OnInteract(Farmer farmer, NPC npc, GameLocation location)
        {
            var args = new InteractEventArgs(farmer, npc, location);

            Interact?.Invoke(this, args);

            return args.IsSupressed;
        }
    }
}
