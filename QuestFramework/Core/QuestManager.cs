using Netcode;
using StardewValley;
using QuestFramework.API;
using QuestFramework.Core.Model;

namespace QuestFramework.Core
{
    internal class QuestManager : IQuestManager, INetObject<NetFields>, IDisposable
    {
        private readonly NetObjectList<ICustomQuest> _quests = new();
        private readonly NetLong _farmerID = new();
        private readonly NetStringList _rules = new();
        private bool _disposedValue;

        public static NetRootDictionary<long, QuestManager> Managers { get; } = new();
        public static Dictionary<string, IQuestProvider> Providers { get; } = new();
        public static QuestManager? Current
        {
            get
            {
                if (Game1.player != null)
                {
                    if (Managers.TryGetValue(Game1.player.UniqueMultiplayerID, out var manager))
                    {
                        return manager;
                    }
                }

                return null;
            }
        }

        public long PlayerId => _farmerID.Value;
        public Farmer Player => Game1.getFarmerMaybeOffline(_farmerID.Value);
        public bool IsActive => Player == Game1.player;

        public NetFields NetFields { get; }

        public IList<ICustomQuest> Quests => _quests;
        public IList<string> Rules => _rules;

        public QuestManager(Farmer player) : this(player.UniqueMultiplayerID)
        {
        }

        public QuestManager(long playerId) : this()
        {
            _farmerID.Value = playerId;
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

        private void OnQuestChanged(NetList<ICustomQuest, NetRef<ICustomQuest>> list, int index, ICustomQuest oldValue, ICustomQuest newValue)
        {
            if (oldValue != null && oldValue != newValue)
            {
                oldValue.OnRemove();
            }

            UpdateQuest(newValue);
        }

        private void OnQuestsReplaced(NetList<ICustomQuest, NetRef<ICustomQuest>> list, IList<ICustomQuest> before, IList<ICustomQuest> after)
        {
            if (before != null)
            {
                foreach (var quest in before)
                {
                    if (quest == null || after.Contains(quest)) { continue; }
                    quest.OnRemove();
                }
            }

            if (after != null)
            {
                foreach (var quest in after)
                {
                    UpdateQuest(quest);
                }
            }
        }

        private void UpdateQuest(ICustomQuest quest)
        {
            if (quest == null) { return; }

            if (quest.Manager != this)
            {
                if (quest.Manager != null)
                {
                    quest.OnRemove();
                }

                quest.OnAdd(this);
            }

            if (!quest.IsAccepted())
            {
                quest.OnAccept();
            }
        }

        public void AddQuest(string questId, int? seed = null, bool ignoreDuplicities = false)
        {
            if (string.IsNullOrWhiteSpace(questId))
            {
                Logger.Warn("No quest ID specified!");
                return;
            }

            if (!ignoreDuplicities && HasQuest(questId))
            {
                Logger.Warn($"Quest with ID '{questId}' is already added.");
                return;
            }

            try
            {
                ICustomQuest? quest = CreateQuest(questId, seed);

                if (quest == null)
                {
                    Logger.Warn($"Can't add quest with ID '{questId}' because no such ID was found.");
                    return;
                }

                AddQuest(quest, ignoreDuplicities);
            }
            catch (QuestException e)
            {
                Logger.Error("Can't add quest", e, stack: false);
            }
        }

        public void AddQuest(ICustomQuest quest, bool allowDuplicity = false)
        {
            if (!allowDuplicity && !string.IsNullOrWhiteSpace(quest.Id) && HasQuest(quest.Id))
            {
                Logger.Warn($"Quest with ID '{quest.Id}' is already added in {Player.Name}'s quest log.");
                return;
            }

            Logger.Trace($"Quest with ID '{quest.Id}' added to {Player.Name}'s quest log.");
            Quests.Add(quest);
        }

        public void RemoveQuest(string questId)
        {
            var quest = Quests.FirstOrDefault(q => q.Id == questId);

            if (quest != null)
            {
                Quests.Remove(quest);
            }
        }

        public bool HasQuest(string questId) => Quests.Any(q => q.Id == questId);

        public void LoadFromState(QuestManagerState managerState)
        {
            _quests.Clear();
            _rules.Clear();
            _rules.CopyFrom(managerState.Rules);
            
            foreach(var quest in managerState.Quests)
            {
                if (quest != null && quest.Reload())
                {
                    _quests.Add(quest);
                }
            }
        }

        public QuestManagerState GetSaveState()
        {
            return new QuestManagerState()
            {
                Quests = _quests.ToList(),
                Rules = _rules.ToList(),
            };
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

            if (Utils.IsQfQuestId(questId))
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

            throw new QuestCreationException($"Quest id '{questId}' is not full quialified");
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
                UpdateQuest(quest);
                quest.Update();
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    foreach (var quest in Quests)
                    {
                        if (quest is not IDisposable disposable) 
                            continue;
                        disposable.Dispose();
                    }
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                _disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~QuestManager()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
