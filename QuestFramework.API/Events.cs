using StardewValley;
using StardewValley.Monsters;

namespace QuestFramework.API.Events
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="Farmer">Farmer who caught the fish</param>
    /// <param name="Fish">The fish caught by farmer</param>
    public record FishCaughtEventArgs(Farmer Farmer, Item Fish);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="Farmer">Farmer who gives a gift</param>
    /// <param name="Receiver">NPC who recieves a gift</param>
    /// <param name="Gift">The given gift item</param>
    public record GiftGivenEventArgs(Farmer Farmer, NPC Receiver, Item Gift);

    public record ItemCollectedEventArgs(Farmer Farmer, Item Item);
    public record ItemDeliveredEventArgs(Farmer Farmer, NPC Receiver, Item Item, bool Probe);
    public record ItemShippedEventArgs(Farmer Farmer, Item Item, int Price);
    public record JKScoreAchievedEventArgs(Farmer Farmer, int Score);
    public record MineFloorReachedEventArgs(Farmer Farmer, int Floor);
    public record MonsterSlainEventArgs(Farmer Farmer, Monster Monster);
}
