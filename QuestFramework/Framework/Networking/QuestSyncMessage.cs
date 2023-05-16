namespace QuestFramework.Framework.Networking
{
    internal class QuestSyncMessage
    {
        public enum SyncType
        {
            DELTA,
            FULL
        }

        public SyncType Type { get; set; }
        public byte[] Data { get; set; }
        public long FarmerID { get; set; }

        public QuestSyncMessage(byte[] data, long farmerID)
        {
            Data = data;
            FarmerID = farmerID;
        }

        public BinaryReader AsReader()
        {
            MemoryStream stream = new(Data);

            return new BinaryReader(stream);
        }
    }
}
