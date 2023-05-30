using StardewValley.Monsters;
using StardewValley;

namespace QuestFramework.Quests.Data
{
    public record ItemMessage(Farmer Farmer, Item Item, int Price = 0);

    public record GiftMessage(Farmer Farmer, NPC NPC, Item Item);

    public record ScoreMessage(Farmer Farmer, int Score);
    public record MineFloorMessage(Farmer Farmer, int Floor);
    public record MonsterMessage(Farmer Farmer, Monster Monster);
}
