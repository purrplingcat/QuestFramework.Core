namespace QuestFramework.Core.Networking
{
    internal class QuestSyncMessage
    {
        public enum SyncType
        {
            DELTA,
            FULL,
            CREATE,
            DISPOSE,
        }

        public SyncType Type { get; set; }
        public byte[] Data { get; set; }
        public long PeerId { get; set; }

        public QuestSyncMessage(byte[] data, long peerId, SyncType type)
        {
            Data = data;
            PeerId = peerId;
            Type = type;
        }

        public BinaryReader AsReader()
        {
            MemoryStream stream = new(Data);

            return new BinaryReader(stream);
        }
    }
}
