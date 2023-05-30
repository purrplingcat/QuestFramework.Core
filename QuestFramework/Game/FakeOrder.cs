using QuestFramework.Extensions;
using QuestFramework.Quests.Data;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Monsters;

namespace QuestFramework.Game
{
    internal class FakeOrder : SpecialOrder
    {
        public static FakeOrder? Me { get; private set; }

        public static void Install()
        {
            if (!Context.IsWorldReady || !Context.IsMainPlayer) { return; }

            Me ??= new FakeOrder();

            if (Context.IsWorldReady && !Game1.player.team.specialOrders.Contains(Me))
            {
                Me.dueDate.Value = int.MaxValue;
                Game1.player.team.specialOrders.Add(Me);
            }
        }

        public static void Uninstall()
        {
            if (!Context.IsWorldReady || !Context.IsMainPlayer) { return; }

            if (Me != null && Game1.player.team.specialOrders.Contains(Me))
            {
                Game1.player.team.specialOrders.Remove(Me);
            }
        }

        public override void Update()
        {
            if (questState.Value != QuestState.InProgress)
            {
                questState.Value = QuestState.InProgress;
            }

            if (participants.Any())
            {
                participants.Clear();
            }
        }

        public void SubscribeEvents()
        {
            onFishCaught += OnFishCaught;
            onGiftGiven += OnGiftGiven;
            onItemCollected += OnItemCollected;
            onItemDelivered += OnItemDelivered;
            onItemShipped += OnItemShipped;
            onJKScoreAchieved += OnJKScoreAchieved;
            onMineFloorReached += OnMineFloorReached;
            onMonsterSlain += OnMonsterSlain;
        }

        private void OnMonsterSlain(Farmer farmer, Monster monster) 
            => SendMessage("MonsterSlain", new MonsterMessage(farmer, monster));

        private void OnMineFloorReached(Farmer farmer, int floor) 
            => SendMessage("MineFloorReached", new MineFloorMessage(farmer, floor));

        private void OnJKScoreAchieved(Farmer farmer, int score) 
            => SendMessage("JKScoreAchieved", new ScoreMessage(farmer, score));

        private void OnItemShipped(Farmer farmer, Item item, int price) 
            => SendMessage("ItemShipped", new ItemMessage(farmer, item, price));

        private int OnItemDelivered(Farmer farmer, NPC nPC, Item item)
        {
            int originalAmount = item.Stack;

            SendMessage("ItemDelivered", new GiftMessage(farmer, nPC, item));

            return originalAmount - item.Stack;
        }

        private void OnItemCollected(Farmer farmer, Item item) 
            => SendMessage("ItemCollected", new ItemMessage(farmer, item));

        private void OnGiftGiven(Farmer farmer, NPC nPC, Item item) 
            => SendMessage("GiftGiven", new GiftMessage(farmer, nPC, item));

        private void OnFishCaught(Farmer farmer, Item item) 
            => SendMessage("FishCaught", new ItemMessage(farmer, item));

        private static void SendMessage<T>(string type, T message) where T : class
        {
            Game1.player.GetQuestManager().CheckQuests(type, message);
        }
    }
}
