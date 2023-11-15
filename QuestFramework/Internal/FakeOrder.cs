using StardewValley;
using StardewValley.SpecialOrders;
using StardewModdingAPI;
using static QuestFramework.QuestCoreMod;

namespace QuestFramework.Internal
{
    internal class FakeOrder : SpecialOrder
    {
        private bool _subscribed;
        public static FakeOrder? Me { get; private set; }

        public static void Install()
        {
            if (!Context.IsWorldReady || !Context.IsMainPlayer) { return; }

            Me ??= new FakeOrder();

            if (!Game1.player.team.specialOrders.Contains(Me))
            {
                Me.SubscribeEvents();
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

        public static void CleanUp()
        {
            Me = null;
        }

        public override void Update()
        {
            if (!_subscribed)
            {
                SubscribeEvents();
            }
            
            if (questState.Value != SpecialOrderStatus.InProgress)
            {
                questState.Value = SpecialOrderStatus.InProgress;
            }

            if (participants.Any())
            {
                participants.Clear();
            }
        }

        public void SubscribeEvents()
        {
            if (_subscribed) { return; }

            _subscribed = true;
            onFishCaught += Events.OnFishCaught;
            onGiftGiven += Events.OnGiftGiven;
            onItemCollected += Events.OnItemCollected;
            onItemDelivered += Events.OnItemDelivered;
            onItemShipped += Events.OnItemShipped;
            onJKScoreAchieved += Events.OnJKScoreAchieved;
            onMineFloorReached += Events.OnMineFloorReached;
            onMonsterSlain += Events.OnMonsterSlain;
        }
    }
}
