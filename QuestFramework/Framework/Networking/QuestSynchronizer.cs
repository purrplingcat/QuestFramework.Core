using Netcode;
using QuestFramework.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace QuestFramework.Framework.Networking
{
    internal class QuestSynchronizer
    {
        private const string MSG_SYNC = "SYNC_QUESTS";

        private readonly IMultiplayerHelper _multiplayer;
        private readonly IManifest _manifest;

        private static QuestFrameworkConfig Config => QuestFrameworkMod.Config;

        public QuestSynchronizer(Mod mod)
        {
            _multiplayer = mod.Helper.Multiplayer;
            _manifest = mod.ModManifest;

            var events = mod.Helper.Events;
            events.Multiplayer.PeerConnected += OnPeerConnected;
            events.Multiplayer.ModMessageReceived += OnMultiplayerMessageReceived;
            events.GameLoop.UpdateTicked += OnGameUpdated;
        }

        private void OnPeerConnected(object? sender, PeerConnectedEventArgs e)
        {
            var mod = e.Peer.GetMod(_multiplayer.ModID);

            if (mod == null || !mod.Version.Equals(_manifest.Version))
            {
                Logger.Error($"Mismatch Quest Framework version for peer ${e.Peer.PlayerID}: {mod?.Version} != {_manifest.Version}");
                return;
            }

            if (!Context.IsMainPlayer) { return; }

            Farmer farmer = Game1.getFarmer(e.Peer.PlayerID);
            if (farmer == null) return;

            var toMods = new string[] { _manifest.UniqueID };
            var toPlayers = new long[] { e.Peer.PlayerID };

            if (!QuestManager.Managers.ContainsKey(e.Peer.PlayerID))
            {
                QuestManager manager = new(farmer);
                QuestManager.Managers.Add(e.Peer.PlayerID, manager);

                var msg = new QuestSyncMessage(Game1.Multiplayer.writeObjectFullBytes(QuestManager.Managers.Roots[e.Peer.PlayerID], e.Peer.PlayerID), e.Peer.PlayerID)
                {
                    Type = QuestSyncMessage.SyncType.FULL
                };

                _multiplayer.SendMessage(msg, MSG_SYNC, toMods);
                Logger.Trace($"(SYNC) Sent newly added Quest Manager for playerID {msg.FarmerID} to all connected peers");
            }

            foreach (var manager in QuestManager.Managers.Roots)
            {
                var msg = new QuestSyncMessage(Game1.Multiplayer.writeObjectFullBytes(manager.Value, e.Peer.PlayerID), manager.Key)
                {
                    Type = QuestSyncMessage.SyncType.FULL
                };

                _multiplayer.SendMessage(msg, MSG_SYNC, toMods, toPlayers);
                Logger.Trace($"(SYNC) Sent Quest Manager full data of playerID {msg.FarmerID} to {string.Join(" ,", toPlayers)}");
            }
        }

        private void OnMultiplayerMessageReceived(object? sender, ModMessageReceivedEventArgs e)
        {
            if (e.Type == MSG_SYNC && e.FromModID == _multiplayer.ModID && e.FromPlayerID != Game1.player.UniqueMultiplayerID)
            {
                var msg = e.ReadAs<QuestSyncMessage>();

                switch (msg.Type)
                {
                    case QuestSyncMessage.SyncType.FULL:
                        QuestManager.Managers.Roots[msg.FarmerID] = Game1.Multiplayer.readObjectFull<QuestManager>(msg.AsReader());
                        Logger.Trace($"(SYNC) Received Quest Manager full-data for playerID: {msg.FarmerID} Source player: {e.FromPlayerID}");
                        break;
                    case QuestSyncMessage.SyncType.DELTA:
                        if (QuestManager.Managers.Roots.TryGetValue(msg.FarmerID, out var delta))
                        {
                            Game1.Multiplayer.readObjectDelta(msg.AsReader(), delta);
                            Logger.Trace($"(SYNC) Received Quest Manager delta-data for playerID: {msg.FarmerID} Source player: {e.FromPlayerID}");
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

                if (Context.IsMainPlayer && e.IsMultipleOf(Config.DeltaBroadcastPeriod))
                {
                    SendQuestDelta(manager.Value, manager.Key);
                }
            }

            if (!Context.IsMainPlayer && e.IsMultipleOf(Config.DeltaBroadcastPeriod))
            {
                if (QuestManager.Managers.Roots.TryGetValue(Game1.player.UniqueMultiplayerID, out var localManager))
                {
                    SendQuestDelta(localManager, Game1.player.UniqueMultiplayerID);
                }
            }
        }

        private void SendQuestDelta(NetRoot<QuestManager> root, long playerId)
        {
            if (Context.IsMultiplayer && root.Dirty)
            {
                var msg = new QuestSyncMessage(Game1.Multiplayer.writeObjectDeltaBytes(root), playerId);
                _multiplayer.SendMessage(msg, MSG_SYNC, new string[] { _manifest.UniqueID });
                root.MarkClean();
                Logger.Trace($"(SYNC) Sent delta update for Quest Manager playerID: {msg.FarmerID}");
            }
        }
    }
}
