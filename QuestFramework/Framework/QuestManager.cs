using Netcode;
using Newtonsoft.Json;
using QuestFramework.API;
using QuestFramework.API.Exceptions;
using QuestFramework.Data;
using QuestFramework.Model;
using QuestFramework.Providers;
using StardewValley;

namespace QuestFramework.Framework
{
    internal class QuestManager : IQuestManager, INetObject<NetFields>
    {
        private readonly NetObjectList<ICustomQuest> _quests = new();
        private readonly NetLong _farmerID = new();

        public static NetRootDictionary<long, QuestManager> Managers { get; } = new();
        public static Dictionary<string, IQuestProvider> Providers { get; } = new();
        public static QuestManager? Current
        {
            get
            {
                if (Managers.TryGetValue(Game1.player.UniqueMultiplayerID, out var manager))
                {
                    return manager;
                }

                return null;
            }
        }

        public long RefId => _farmerID.Value;
        public Farmer Player => Game1.getFarmerMaybeOffline(_farmerID.Value);
        public bool IsActive => Player == Game1.player;

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
            _quests.OnElementChanged += OnQuestChanged;
            _quests.OnArrayReplaced += OnQuestsReplaced;
        }

        private void OnQuestChanged(NetList<ICustomQuest, NetRef<ICustomQuest>> list, int index, ICustomQuest oldValue, ICustomQuest newValue) => HookOnQuest(newValue);

        private void OnQuestsReplaced(NetList<ICustomQuest, NetRef<ICustomQuest>> list, IList<ICustomQuest> before, IList<ICustomQuest> after)
        {
            if (before != null)
            {
                foreach (var quest in before)
                {
                    if (after.Contains(quest)) { continue; }
                    quest.OnRemoved();
                }
            }

            if (after != null)
            {
                foreach (var quest in after)
                {
                    HookOnQuest(quest);
                }
            }
        }

        private void HookOnQuest(ICustomQuest quest)
        {
            if (quest.Manager != this)
            {
                if (quest.Manager != null)
                {
                    quest.OnRemoved();
                }

                quest.OnAdd(this);
            }

            if (quest.IsAccepted())
            {
                quest.OnAccept();
            }
        }

        public void AddQuest(string questId, int? seed = null)
        {
            try
            {
                ICustomQuest? quest = CreateQuest(questId, seed);

                if (quest == null)
                {
                    Logger.Warn($"Can't add quest with ID '{questId}' because no such ID was found.");
                    return;
                }

                AddQuest(quest);
            }
            catch (QuestException e)
            {
                Logger.Error("Can't add quest", e, stack: false);
            }
        }

        public void AddQuest(ICustomQuest quest)
        {
            if (!string.IsNullOrWhiteSpace(quest.Id) && Quests.Any(q => q.Id == quest.Id))
            {
                Logger.Warn($"Quest with ID '{quest.Id}' is already added.");
                return;
            }

            Quests.Add(quest);
        }

        public void LoadState(QuestManagerState managerState, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public QuestManagerState SaveState(JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public static ICustomQuest? CreateQuest(string questId, int? seed = null)
        {
            if (string.IsNullOrWhiteSpace(questId))
            {
                throw new QuestCreationException(questId, "Quest ID can't be empty!");
            }

            if (questId.StartsWith("#"))
            {
                throw new QuestCreationException(questId, "ID prefix '#' is reserved for automatic generated quests.");
            }

            if (questId[0] == '(' && questId.Contains(')'))
            {
                int splitIndex = questId.IndexOf(')');
                string qualifier = questId[..(splitIndex + 1)];
                QuestMetadata questMetadata = new()
                {
                    QualifiedId = questId,
                    LocalId = questId.Replace(qualifier, ""),
                    TypeIdentifier = qualifier[1..(qualifier.Length - 1)],
                    Seed = seed ?? Game1.random.Next(),
                };

                return CreateQuest(questMetadata);
            }

            return CreateQuest($"(Q){questId}");
        }

        public static ICustomQuest? CreateQuest(IQuestMetadata questMetadata)
        {
            if (!Providers.TryGetValue(questMetadata.TypeIdentifier, out var provider))
            {
                throw new QuestException($"No provider found for quest type identifier: {questMetadata.TypeIdentifier}");
            }

            return provider.CreateQuest(questMetadata);
        }

        public void Update()
        {
            if (!IsActive) { return; }

            foreach (var quest in Quests)
            {
                HookOnQuest(quest);
                quest.Update();
            }
        }
    }
}
