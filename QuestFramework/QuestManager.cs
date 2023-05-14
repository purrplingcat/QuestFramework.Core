using Netcode;
using QuestFramework.API;
using StardewValley;

namespace QuestFramework
{
    internal class QuestManager : INetObject<NetFields>
    {   
        private readonly NetObjectList<ICustomQuest> _quests = new();
        private readonly NetLong _farmerID = new();

        public static NetRootDictionary<long, QuestManager> Managers { get; } = new();
        public Farmer Player => Game1.getFarmerMaybeOffline(_farmerID.Value);

        public NetFields NetFields { get; }

        public IList<ICustomQuest> Quests => _quests;

        public QuestManager(Farmer player) : this()
        {
            _farmerID.Value = player.UniqueMultiplayerID;
        }

        public QuestManager() 
        {
            NetFields = new NetFields(NetFields.GetNameForInstance(this));
            InitNetFields();
        }

        private void InitNetFields()
        {
            NetFields.SetOwner(this)
                .AddField(_farmerID, "farmerID")
                .AddField(_quests, "quests");
        }

        public void LoadState(QuestManagerState managerState)
        {
            throw new NotImplementedException();
        }
    }
}
