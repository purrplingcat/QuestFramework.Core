using StardewValley;
using StardewValley.Monsters;

namespace QuestFramework.Core.Events
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
    
    public abstract class SupressableEvent
    {
        public bool IsSupressed { get; private set; }

        public void Supress()
        {
            IsSupressed = true;
        }
    }

    /// <summary>
    /// When a farmer tries talk to an NPC, this cointains the information.
    /// This event can be supressed and avoid trig the vanilla function of <see cref="NPC.checkAction(Farmer, GameLocation)"/>,
    /// but mods may ignore the event supression, if they don't check the supression flag.
    /// </summary>
    public class InteractEventArgs : SupressableEvent
    {
        public InteractEventArgs(Farmer farmer, NPC npc, GameLocation location)
        {
            Farmer = farmer ?? throw new ArgumentNullException(nameof(farmer));
            NPC = npc ?? throw new ArgumentNullException(nameof(npc));
            Location = location ?? throw new ArgumentNullException(nameof(location));
        }

        public Farmer Farmer { get; }
        public NPC NPC { get; }
        public GameLocation Location { get; }
        
    }
}
