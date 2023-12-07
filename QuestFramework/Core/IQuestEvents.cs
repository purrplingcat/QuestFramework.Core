using QuestFramework.Core.Events;

namespace QuestFramework.Core
{
    public interface IQuestEvents
    {
        event EventHandler<FishCaughtEventArgs> FishCaught;
        event EventHandler<GiftGivenEventArgs> GiftGiven;
        event EventHandler<ItemCollectedEventArgs> ItemCollected;
        event EventHandler<ItemDeliveredEventArgs> ItemDelivered;
        event EventHandler<ItemShippedEventArgs> ItemShipped;
        event EventHandler<JKScoreAchievedEventArgs> JKScoreAchieved;
        event EventHandler<MineFloorReachedEventArgs> MineFloorReached;
        event EventHandler<MonsterSlainEventArgs> MonsterSlain;
        event EventHandler<InteractEventArgs> Interact;
    }
}
