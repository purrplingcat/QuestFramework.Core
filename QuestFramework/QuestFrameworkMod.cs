using Newtonsoft.Json;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace QuestFramework
{
    public class QuestFrameworkMod : Mod
    {
        private const string MSG_SYNC = "SYNC_QUESTS";
        private readonly JsonSerializerSettings _jsonSerializerSettings = new();

        public override void Entry(IModHelper helper)
        {
            helper.Events.Multiplayer.PeerConnected += OnPeerConnected;
            helper.Events.Multiplayer.ModMessageReceived += OnMultiplayerMessageReceived;
            helper.Events.GameLoop.UpdateTicked += OnGameUpdated;
            helper.Events.GameLoop.SaveLoaded += OnSaveLoaded;
            helper.Events.GameLoop.Saving += OnSaving;
        }

        private void OnSaving(object? sender, SavingEventArgs e)
        {
            if (!Context.IsMainPlayer) { return; }

            var serializer = JsonSerializer.Create(_jsonSerializerSettings);
            var save = new QuestFrameworkState
            {
                Version = ModManifest.Version,
                Quests = QuestManager.Managers
                .ToDictionary((m) => m.Key, (m) => m.Value.SaveState(serializer))
            };

            Helper.Data.WriteSaveData("Quests", save);
        }

        private void OnSaveLoaded(object? sender, SaveLoadedEventArgs e)
        {
            if (!Context.IsMainPlayer) { return; }

            var serializer = JsonSerializer.Create(_jsonSerializerSettings);
            QuestFrameworkState save = Helper.Data.ReadSaveData<QuestFrameworkState>("Quests") ?? new();

            foreach(var farmer in Game1.getAllFarmers())
            {
                var manager = new QuestManager(farmer);

                if (save.Quests.TryGetValue(farmer.UniqueMultiplayerID,out var managerState))
                {
                    manager.LoadState(managerState, serializer);
                }

                QuestManager.Managers.Add(farmer.UniqueMultiplayerID, manager);
            }
        }

        private void OnPeerConnected(object? sender, PeerConnectedEventArgs e)
        {
            if (!Context.IsMainPlayer) { return; }
           
            var mod = e.Peer.GetMod(Helper.Multiplayer.ModID);

            if (mod == null || !mod.Version.Equals(ModManifest.Version))
            {
                Game1.server.kick(e.Peer.PlayerID);
                Monitor.Log($"Mismatch Quest Framework version for peer ${e.Peer.PlayerID}: {mod?.Version} != {ModManifest.Version}", LogLevel.Error);
                return;
            }

            Farmer farmer = Game1.getFarmer(e.Peer.PlayerID); 
            if (farmer == null) return;

            if (!QuestManager.Managers.ContainsKey(e.Peer.PlayerID))
            {
                QuestManager.Managers.Add(e.Peer.PlayerID, new QuestManager(farmer));
            }

            var toMods = new string[] { ModManifest.UniqueID };
            var toPlayers = new long[] { e.Peer.PlayerID };

            foreach (var manager in QuestManager.Managers.Roots)
            {
                var msg = new QuestSyncMessage(Game1.Multiplayer.writeObjectFullBytes(manager.Value, e.Peer.PlayerID), manager.Key) 
                { 
                    Type = QuestSyncMessage.SyncType.FULL 
                };

                Helper.Multiplayer.SendMessage(msg, MSG_SYNC, toMods, toPlayers);
            }
        }

        private void OnMultiplayerMessageReceived(object? sender, ModMessageReceivedEventArgs e)
        {
            if (e.Type == MSG_SYNC && e.FromModID == Helper.Multiplayer.ModID && e.FromPlayerID != Game1.player.UniqueMultiplayerID)
            {
                var msg = e.ReadAs<QuestSyncMessage>();
                
                switch (msg.Type)
                {
                    case QuestSyncMessage.SyncType.FULL:
                        QuestManager.Managers.Roots[msg.FarmerID] = Game1.Multiplayer.readObjectFull<QuestManager>(msg.AsReader());
                        break;
                    case QuestSyncMessage.SyncType.DELTA:
                        if (QuestManager.Managers.Roots.TryGetValue(msg.FarmerID, out var delta))
                        {
                            Game1.Multiplayer.readObjectDelta(msg.AsReader(), delta);
                        }
                        break;
                }
            }
        }

        private void OnGameUpdated(object? sender, UpdateTickedEventArgs e)
        {
            if (e.IsMultipleOf(10))
            {
                var toMods = new string[] { ModManifest.UniqueID };

                foreach (var manager in QuestManager.Managers.Roots)
                {
                    if (manager.Value.Dirty)
                    {
                        var msg = new QuestSyncMessage(Game1.Multiplayer.writeObjectDeltaBytes(manager.Value), manager.Key);
                        Helper.Multiplayer.SendMessage(msg, MSG_SYNC, toMods);
                        manager.Value.MarkClean();
                    }
                    Game1.Multiplayer.updateRoot(manager.Value);
                }
            }
        }
    }
}
