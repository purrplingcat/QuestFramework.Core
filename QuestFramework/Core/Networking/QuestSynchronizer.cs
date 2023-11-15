using Netcode;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using static QuestFramework.Core.Networking.QuestSyncMessage;

namespace QuestFramework.Core.Networking
{
    internal class QuestSynchronizer
    {
        private const string MSG_TYPE = "NET_SYNC_QUESTS";

        private readonly IMultiplayerHelper _multiplayer;
        private readonly IManifest _manifest;

        public NetRootDictionary<long, QuestManager> Peers { get; set; }
        public static long PlayerId => Game1.player?.UniqueMultiplayerID ?? 0L;

        private static QuestFrameworkConfig Config => QuestCoreMod.Config;

        public QuestSynchronizer(Mod mod, NetRootDictionary<long, QuestManager> peers)
        {
            Peers = peers;
            _multiplayer = mod.Helper.Multiplayer;
            _manifest = mod.ModManifest;
            
            var events = mod.Helper.Events;
            events.Multiplayer.PeerConnected += OnPeerConnected;
            events.Multiplayer.PeerDisconnected += OnPeerDisconnected;
            events.Multiplayer.ModMessageReceived += OnMultiplayerMessageReceived;
            events.GameLoop.UpdateTicked += OnGameUpdated;
            events.GameLoop.SaveLoaded += OnGameLoaded;
        }

        private void OnPeerDisconnected(object? sender, PeerDisconnectedEventArgs e)
        {
            if (!Context.IsWorldReady) { return; }

            foreach (var peer in Peers.Roots.Values)
            {
                peer.Disconnect(e.Peer.PlayerID);
            }
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

            if (!Peers.ContainsKey(e.Peer.PlayerID))
            {
                Peers.Add(e.Peer.PlayerID, new QuestManager(farmer));
            }

            SendAllPeers(e.Peer.PlayerID);
            Introduce(Peers.Roots[e.Peer.PlayerID], e.Peer.PlayerID);
        }

        private void SendAllPeers(long toPeerId)
        {
            foreach (var peer in Peers.Roots)
            {
                var msg = new QuestSyncMessage(
                    Game1.Multiplayer.writeObjectFullBytes(peer.Value, toPeerId),
                    peer.Key,
                    SyncType.FULL
                );

                SendMessage(msg, toPeerId);
                Logger.Trace($"(SYNC) Sent Quest Manager full data of playerID {msg.PeerId} to {toPeerId}");
            }
        }

        private void Introduce(NetRoot<QuestManager> peer, long peerId)
        {
            var msg = new QuestSyncMessage(
                Game1.Multiplayer.writeObjectFullBytes(peer, peerId), 
                peer.Value.PlayerId,
                SyncType.FULL
            );

            SendMessage(msg);
            Logger.Trace($"(SYNC) Sent Quest Manager full data of playerID {msg.PeerId} to all players");
        }

        private void OnMultiplayerMessageReceived(object? sender, ModMessageReceivedEventArgs e)
        {
            if (e.Type == MSG_TYPE && e.FromModID == _multiplayer.ModID && e.FromPlayerID != PlayerId)
            {
                var msg = e.ReadAs<QuestSyncMessage>();

                switch (msg.Type)
                {
                    case SyncType.FULL:
                        ReadFull(e.FromPlayerID, msg.PeerId, msg.AsReader());
                        break;
                    case SyncType.DELTA:
                        ReadDelta(e.FromPlayerID, msg.PeerId, msg.AsReader());
                        break;
                    case SyncType.CREATE:
                        ReadCreate(e.FromPlayerID, msg.PeerId); 
                        break;
                    case SyncType.DISPOSE:
                        ReadDispose(e.FromPlayerID, msg.PeerId);
                        break;
                }
            }
        }

        private void ReadCreate(long sourceId, long peerId)
        {
            if (!Peers.ContainsKey(peerId))
            {
                Peers.Add(peerId, new QuestManager(peerId));
                Logger.Trace($"(SYNC) Received Quest Manager create request for playerID: {peerId} Source player: {sourceId}");
            }
        }

        private void ReadDispose(long sourceId, long peerId)
        {
            if (Peers.ContainsKey(peerId))
            {
                if (Peers[peerId] is IDisposable disposable)
                {
                    disposable.Dispose();
                }

                Peers.Remove(peerId);
                Logger.Trace($"(SYNC) Received Quest Manager disposal request for playerID: {peerId} Source player: {sourceId}");
            }
        }

        private void ReadDelta(long sourceId, long peerId, BinaryReader reader)
        {
            if (Peers.Roots.TryGetValue(peerId, out var delta))
            {
                Game1.Multiplayer.readObjectDelta(reader, delta);
                Logger.Trace($"(SYNC) Received Quest Manager delta-data for playerID: {peerId} Source player: {sourceId}");
            }
        }

        private void ReadFull(long sourceId, long peerId, BinaryReader reader)
        {
            Peers.Roots[peerId] = Game1.Multiplayer.readObjectFull<QuestManager>(reader);
            Logger.Trace($"(SYNC) Received Quest Manager full-data for playerID: {peerId} Source player: {sourceId}");
        }

        private static bool CanSyncNow(UpdateTickedEventArgs e) 
            => !Game1.Multiplayer.allowSyncDelay() || e.IsMultipleOf(Config.DeltaBroadcastPeriod);

        [EventPriority(EventPriority.Low - 1000)]
        private void OnGameUpdated(object? sender, UpdateTickedEventArgs e)
        {
            if (!Context.IsWorldReady) { return; }

            foreach ((long peerId, NetRoot<QuestManager> peer) in Peers.Roots)
            {
                peer.Clock.InterpolationTicks = Game1.Multiplayer.interpolationTicks();
                Game1.Multiplayer.updateRoot(peer);

                if (Context.IsMainPlayer && CanSyncNow(e))
                {
                    SendDelta(peer, peerId);
                }
            }

            if (!Context.IsMainPlayer && CanSyncNow(e))
            {
                if (Peers.Roots.TryGetValue(PlayerId, out var localManager))
                {
                    SendDelta(localManager, PlayerId);
                }
            }
        }

        private void SendDelta(NetRoot<QuestManager> root, long playerId)
        {
            if (Context.IsMultiplayer && root.Dirty)
            {
                var msg = new QuestSyncMessage(
                    Game1.Multiplayer.writeObjectDeltaBytes(root), 
                    playerId, 
                    SyncType.DELTA
                );
                SendMessage(msg);
                root.MarkClean();
                Logger.Trace($"(SYNC) Sent delta update for Quest Manager playerID: {msg.PeerId}");
            }
        }

        public void OnGameLoaded(object? sender, SaveLoadedEventArgs e)
        {
            if (!Context.IsWorldReady || !Context.IsMainPlayer) { return; }

            Game1.netWorldState.Value.farmhandData.OnValueAdded += (long key, Farmer farmer) =>
            {
                if (Peers.ContainsKey(key))
                {
                    Peers.Add(key, new QuestManager(farmer));
                    SendMessage(new QuestSyncMessage(Array.Empty<byte>(), key, SyncType.CREATE));
                }
            };
            Game1.netWorldState.Value.farmhandData.OnValueRemoved += (long key, Farmer value) =>
            {
                if (Peers.ContainsKey(key))
                {
                    Peers.Remove(key);
                    SendMessage(new QuestSyncMessage(Array.Empty<byte>(), key, SyncType.DISPOSE));
                }
            };
        }

        protected void SendMessage(QuestSyncMessage message, long toPeerId) 
            => SendMessage(message, new[] { toPeerId });

        protected virtual void SendMessage(QuestSyncMessage message, long[]? toPeerIds = null)
        {
            if (Context.IsMultiplayer)
            {
                _multiplayer.SendMessage(message, MSG_TYPE, new string[] { _manifest.UniqueID }, toPeerIds);
            }
        }
    }
}
