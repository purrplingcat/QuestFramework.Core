using Netcode;
using Newtonsoft.Json;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace QuestFramework
{
    public class QuestFrameworkMod : Mod
    {
        private const string MSG_SYNC = "SYNC_QUESTS";
        private const string SAVE_KEY = "Quests";

        private readonly JsonSerializerSettings _jsonSerializerSettings = new();

        public override void Entry(IModHelper helper)
        {
            helper.Events.Multiplayer.PeerConnected += OnPeerConnected;
            helper.Events.Multiplayer.ModMessageReceived += OnMultiplayerMessageReceived;
            helper.Events.GameLoop.UpdateTicked += OnGameUpdated;
            helper.Events.GameLoop.SaveLoaded += OnSaveLoaded;
            helper.Events.GameLoop.Saving += OnSaving;
            helper.Events.GameLoop.ReturnedToTitle += OnExitToTitle;
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

            Helper.Data.WriteSaveData(SAVE_KEY, save);
            Monitor.Log("Quest states for all Quest Managers were successfully saved!", LogLevel.Info);
        }

        private void OnSaveLoaded(object? sender, SaveLoadedEventArgs e)
        {
            if (!Context.IsMainPlayer) { return; }

            var serializer = JsonSerializer.Create(_jsonSerializerSettings);
            QuestFrameworkState save = Helper.Data.ReadSaveData<QuestFrameworkState>(SAVE_KEY) ?? new();

            foreach(var farmer in Game1.getAllFarmers())
            {
                var manager = new QuestManager(farmer);

                if (save.Quests.TryGetValue(farmer.UniqueMultiplayerID,out var managerState))
                {
                    manager.LoadState(managerState, serializer);
                }

                QuestManager.Managers.Add(farmer.UniqueMultiplayerID, manager);
                Monitor.Log($"Loaded Quest Manager state for player '{farmer.Name}' ({farmer.UniqueMultiplayerID})", LogLevel.Debug);
            }

            Monitor.Log($"Successully initialized {QuestManager.Managers.Count} Quest Managers", LogLevel.Info);
        }

        private void OnPeerConnected(object? sender, PeerConnectedEventArgs e)
        {
            if (!Context.IsMainPlayer) { return; }
           
            var mod = e.Peer.GetMod(Helper.Multiplayer.ModID);

            if (mod == null || !mod.Version.Equals(ModManifest.Version))
            {
                Game1.Multiplayer.sendChatMessage(
                    LocalizedContentManager.CurrentLanguageCode, 
                    Helper.Translation.Get(
                        "multiplayer.missMatchVersion", 
                        new { ClientVersion = mod?.Version, ServerVersion = ModManifest.Version }), 
                    e.Peer.PlayerID);
                Monitor.Log($"Mismatch Quest Framework version for peer ${e.Peer.PlayerID}: {mod?.Version} != {ModManifest.Version}", LogLevel.Error);
                return;
            }

            Farmer farmer = Game1.getFarmer(e.Peer.PlayerID); 
            if (farmer == null) return;

            var toMods = new string[] { ModManifest.UniqueID };
            var toPlayers = new long[] { e.Peer.PlayerID };

            if (!QuestManager.Managers.ContainsKey(e.Peer.PlayerID))
            {
                QuestManager manager = new(farmer);
                QuestManager.Managers.Add(e.Peer.PlayerID, manager);

                var msg = new QuestSyncMessage(Game1.Multiplayer.writeObjectFullBytes(QuestManager.Managers.Roots[e.Peer.PlayerID], e.Peer.PlayerID), e.Peer.PlayerID)
                {
                    Type = QuestSyncMessage.SyncType.FULL
                };
                
                Helper.Multiplayer.SendMessage(msg, MSG_SYNC, toMods);
                Monitor.Log($"(SYNC) Sent newly added Quest Manager for playerID {msg.FarmerID} to all connected peers");
            }

            foreach (var manager in QuestManager.Managers.Roots)
            {
                var msg = new QuestSyncMessage(Game1.Multiplayer.writeObjectFullBytes(manager.Value, e.Peer.PlayerID), manager.Key) 
                { 
                    Type = QuestSyncMessage.SyncType.FULL 
                };

                Helper.Multiplayer.SendMessage(msg, MSG_SYNC, toMods, toPlayers);
                Monitor.Log($"(SYNC) Sent Quest Manager full data of playerID {msg.FarmerID} to {string.Join(" ,", toPlayers)}");
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
                        Monitor.Log($"(SYNC) Received Quest Manager full-data for playerID: {msg.FarmerID} Source player: {e.FromPlayerID}");
                        break;
                    case QuestSyncMessage.SyncType.DELTA:
                        if (QuestManager.Managers.Roots.TryGetValue(msg.FarmerID, out var delta))
                        {
                            Game1.Multiplayer.readObjectDelta(msg.AsReader(), delta);
                            Monitor.Log($"(SYNC) Received Quest Manager delta-data for playerID: {msg.FarmerID} Source player: {e.FromPlayerID}");
                        }
                        break;
                }
            }
        }

        private void OnGameUpdated(object? sender, UpdateTickedEventArgs e)
        {
            if (!Context.IsWorldReady) { return; }

            foreach (var manager in QuestManager.Managers.Roots)
            {
                Game1.Multiplayer.updateRoot(manager.Value);

                if (Context.IsMainPlayer && e.IsMultipleOf(5))
                {
                    SendQuestDelta(manager.Value, manager.Key);
                }
            }

            if (!Context.IsMainPlayer && e.IsMultipleOf(5))
            {
                if (QuestManager.Managers.Roots.TryGetValue(Game1.player.UniqueMultiplayerID, out var localManager))
                {
                    SendQuestDelta(localManager, Game1.player.UniqueMultiplayerID);
                }
            }
        }

        private void OnExitToTitle(object? sender, ReturnedToTitleEventArgs e)
        {
            foreach (var manager in QuestManager.Managers.Values)
            {
                if (manager is IDisposable disbosable)
                {
                    disbosable.Dispose();
                }
            }

            QuestManager.Managers.Clear();
            Monitor.Log("Quest Managers were uninitialized", LogLevel.Info);
        }

        private void SendQuestDelta(NetRoot<QuestManager> manager_root, long playerId)
        {
            if (Context.IsMultiplayer && manager_root.Dirty)
            {
                var msg = new QuestSyncMessage(Game1.Multiplayer.writeObjectDeltaBytes(manager_root), playerId);
                Helper.Multiplayer.SendMessage(msg, MSG_SYNC, new string[] { ModManifest.UniqueID });
                manager_root.MarkClean();
                Monitor.Log($"(SYNC) Sent delta update for Quest Manager playerID: {msg.FarmerID}");
            }
        }
    }
}
