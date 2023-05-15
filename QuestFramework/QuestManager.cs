using Netcode;
using Newtonsoft.Json;
using QuestFramework.API;
using StardewValley;

namespace QuestFramework
{
    internal class QuestManager : INetObject<NetFields>
    {   
        private readonly NetObjectList<ICustomQuest> _quests = new();
        private readonly NetLong _farmerID = new();

        public static NetRootDictionary<long, QuestManager> Managers { get; } = new();
        public static Dictionary<string, IQuestProvider> Providers { get; } = new();

        public Farmer Player => Game1.getFarmerMaybeOffline(_farmerID.Value);

        public NetFields NetFields { get; }

        public IList<ICustomQuest> Quests => _quests;

        public QuestManager(Farmer player) : this()
        {
            _farmerID.Value = player.UniqueMultiplayerID;
        }

        static QuestManager()
        {
            Providers.Add("Q", new DefaultQuestProvider());
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

        public void LoadState(QuestManagerState managerState, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        internal QuestManagerState SaveState(JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public static ICustomQuest CreateQuest(string questId)
        {
            if (questId[0] == '(' && questId.Contains(')'))
            {
                int splitIndex = questId.IndexOf(')');
                string providerType = questId.Substring(0, splitIndex + 1);

                if (!Providers.TryGetValue(providerType[1..(providerType.Length - 1)], out var provider))
                {
                    throw new Exception($"No provider found for type: {providerType}");
                }

                return provider.CreateQuest(questId.Replace(providerType, ""));
            }

            return CreateQuest($"(Q){questId}");
        }
    }
}
