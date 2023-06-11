namespace QuestFramework.Framework.Networking
{
    internal class QuestSyncMessage
    {
        public enum SyncType
        {
            DELTA,
            FULL,
            DISPOSE,
        }

        public SyncType Type { get; set; }
        public byte[] Data { get; set; }
        public long PeerId { get; set; }

        public QuestSyncMessage(byte[] data, long peerId)
        {
            Data = data;
            PeerId = peerId;
        }

        public BinaryReader AsReader()
        {
            MemoryStream stream = new(Data);

            return new BinaryReader(stream);
        }
    }
}
