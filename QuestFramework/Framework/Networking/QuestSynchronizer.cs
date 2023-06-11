using Netcode;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using static QuestFramework.Framework.Networking.QuestSyncMessage;

namespace QuestFramework.Framework.Networking
{
    internal class QuestSynchronizer
    {
        private const string MSG_TYPE = "NET_SYNC_QUESTS";

        private readonly IMultiplayerHelper _multiplayer;
        private readonly IManifest _manifest;

        public NetRootDictionary<long, QuestManager> Peers { get; set; }
        public static long PlayerId => Game1.player?.UniqueMultiplayerID ?? 0L;

        private static QuestFrameworkConfig Config => QuestFrameworkMod.Config;

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

            foreach ((long peerId, NetRoot<QuestManager> peer) in Peers.Roots)
            {
                peer.Clock.InterpolationTicks = Game1.Multiplayer.interpolationTicks();
                Game1.Multiplayer.updateRoot(peer);

                if (Context.IsMainPlayer)
                {
                    SendDelta(peer, peerId);
                }
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

            var toMods = new string[] { _manifest.UniqueID };
            var toPlayers = new long[] { e.Peer.PlayerID };

            if (!Peers.ContainsKey(e.Peer.PlayerID))
            {
                AddPeer(e.Peer.PlayerID, new QuestManager(farmer));
            }

            foreach (var peer in Peers.Roots)
            {
                var msg = new QuestSyncMessage(Game1.Multiplayer.writeObjectFullBytes(peer.Value, e.Peer.PlayerID), peer.Key)
                {
                    Type = SyncType.FULL
                };

                _multiplayer.SendMessage(msg, MSG_TYPE, toMods, toPlayers);
                Logger.Trace($"(SYNC) Sent Quest Manager full data of playerID {msg.PeerId} to {string.Join(" ,", toPlayers)}");
            }
        }

        public void AddPeer(long peerId, QuestManager manager)
        {
            Peers.Add(peerId, manager);

            var msg = new QuestSyncMessage(Game1.Multiplayer.writeObjectFullBytes(Peers.Roots[peerId], peerId), peerId)
            {
                Type = SyncType.FULL
            };

            _multiplayer.SendMessage(msg, MSG_TYPE, new string[] { _manifest.UniqueID });
            Logger.Trace($"(SYNC) Sent newly added Quest Manager for playerID {msg.PeerId} to all connected peers");
        }

        private void OnMultiplayerMessageReceived(object? sender, ModMessageReceivedEventArgs e)
        {
            if (e.Type == MSG_TYPE && e.FromModID == _multiplayer.ModID && e.FromPlayerID != PlayerId)
            {
                var msg = e.ReadAs<QuestSyncMessage>();

                switch (msg.Type)
                {
                    case QuestSyncMessage.SyncType.FULL:
                        ReadFull(e.FromPlayerID, msg.PeerId, msg.AsReader());
                        break;
                    case QuestSyncMessage.SyncType.DELTA:
                        ReadDelta(e.FromPlayerID, msg.PeerId, msg.AsReader());
                        break;
                    case QuestSyncMessage.SyncType.DISPOSE:
                        ReadDispose(e.FromPlayerID, msg.PeerId);
                        break;
                }
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
                var msg = new QuestSyncMessage(Game1.Multiplayer.writeObjectDeltaBytes(root), playerId);
                _multiplayer.SendMessage(msg, MSG_TYPE, new string[] { _manifest.UniqueID });
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
                    AddPeer(key, new QuestManager(farmer));
                }
            };
            Game1.netWorldState.Value.farmhandData.OnValueRemoved += (long key, Farmer value) =>
            {
                if (Peers.ContainsKey(key))
                {
                    RemovePeer(key);
                }
            };
        }

        public void RemovePeer(long peerId)
        {
            if (Peers[peerId] is IDisposable disposable)
            {
                disposable.Dispose();
            }

            Peers.Remove(peerId);

            var msg = new QuestSyncMessage(Array.Empty<byte>(), peerId)
            {
                Type = SyncType.DISPOSE
            };
            _multiplayer.SendMessage(msg, MSG_TYPE, new string[] { _manifest.UniqueID });
            Logger.Trace($"(SYNC) Disposal request sent for Quest Manager for playerID {msg.PeerId} to all connected peers");
        }
    }
}
