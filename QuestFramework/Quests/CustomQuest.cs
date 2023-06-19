using Netcode;
using Newtonsoft.Json;
using QuestFramework.API;
using QuestFramework.Quests.Objectives;
using StardewValley;
using System.Runtime.Serialization;

namespace QuestFramework.Quests
{
    public abstract class CustomQuest : ICustomQuest, IHaveObjectives
    {
        public enum QuestState
        {
            InProgress,
            Complete,
            Failed
        }

        private readonly List<QuestObjective> _registeredObjectives = new();
        protected readonly NetString id = new();
        protected readonly NetString questKey = new("");
        protected readonly NetString typeDefinitionId = new("");
        protected readonly NetString name = new();
        protected readonly NetString description = new();
        protected readonly NetEnum<QuestState> state = new(QuestState.InProgress);
        protected readonly NetObjectList<QuestObjective> objectives = new();
        protected readonly NetBool showNew = new();
        protected readonly NetString preconditionsQueryString = new();
        protected readonly NetString preconditionsDescription = new();
        protected bool _objectivesRegistrationDirty;
        protected string? _translatedDescription;
        protected string? _translatedName;

        [JsonProperty("id")]
        public string Id
        {
            get => id.Value;
            set => id.Value = value;
        }

        [JsonProperty("quest_key")]
        public string QuestKey 
        { 
            get => questKey.Value; 
            set => questKey.Value = value; 
        }

        [JsonProperty("type_definition_id")]
        public string TypeDefinitionId 
        { 
            get => typeDefinitionId.Value;
            set => typeDefinitionId.Value = value;
        }

        [JsonProperty("name")]
        public string Name
        {
            get => name.Value;
            set => name.Value = value;
        }

        [JsonProperty("description")]
        public string Description
        {
            get => description.Value;
            set => description.Value = value;
        }

        [JsonProperty("state")]
        public QuestState State
        {
            get => state.Value;
            set => state.Value = value;
        }

        [JsonProperty("show_new")]
        public bool ShowNew
        {
            get => showNew.Value;
            set => showNew.Value = value;
        }

        [JsonProperty("preconditions_query")]
        public string PreconditionsQueryString
        {
            get => preconditionsQueryString.Value;
            set => preconditionsQueryString.Value = value;
        }

        [JsonProperty("preconditions_description")]
        public string PreconditionsDescription
        {
            get => preconditionsDescription.Value;
            set => preconditionsDescription.Value = value;
        }

        [JsonIgnore]
        public IQuestManager? Manager { get; private set; }

        [JsonIgnore]
        public NetFields NetFields { get; }

        [JsonProperty("objectives")]
        public IList<QuestObjective> Objectives
        {
            get => objectives;
            set => objectives.Set(value);
        }

        public CustomQuest() 
        {
            NetFields = new NetFields(NetFields.GetNameForInstance(this));
            InitNetFields(NetFields);
            Initialize();
            Reload();
        }

        public CustomQuest(string id, string questKey = "", string typeDefinitionId = "") : this()
        {
            Id = id;
            TypeDefinitionId = typeDefinitionId;
            QuestKey = questKey;
        }

        protected virtual void InitNetFields(NetFields netFields)
        {
            netFields.SetOwner(this)
                .AddField(id, "id")
                .AddField(questKey, "questKey")
                .AddField(typeDefinitionId, "typeDefinitionId")
                .AddField(state, "state")
                .AddField(objectives, "objectives");

            objectives.OnArrayReplaced += delegate
            {
                _objectivesRegistrationDirty = true;
            };
            objectives.OnElementChanged += delegate
            {
                _objectivesRegistrationDirty = true;
            };
        }

        public abstract bool IsAccepted();
        public abstract bool CanBeCancelled();
        public abstract void MarkAsViewed();
        public abstract bool ShouldDisplayAsComplete();
        public abstract bool IsTimedQuest();
        public abstract int GetDaysLeft();
        public abstract bool IsHidden();
        public abstract bool HasReward();
        public abstract bool HasMoneyReward();
        public abstract int GetMoneyReward();
        public abstract void OnMoneyRewardClaimed();
        public abstract bool OnLeaveQuestPage();
        public abstract void OnAccept();
        public abstract bool Reload();
        protected abstract void Initialize();

        public string GetName()
        {
            _translatedName ??= ParseAndLocalizeText(Name);

            return _translatedName;
        }

        public string GetDescription()
        {
            _translatedDescription ??= ParseAndLocalizeText(Description);

            return _translatedDescription;
        }

        public virtual bool ShouldDisplayAsNew()
        {
            return ShowNew;
        }

        public virtual void Update()
        {
            if (_objectivesRegistrationDirty)
            {
                _objectivesRegistrationDirty = false;
                UpdateObjectiveRegistration();
            }
        }

        public virtual void OnAdd(IQuestManager manager)
        {
            Manager = manager;
        }

        public virtual void OnRemove()
        {
            Manager = null;
        }

        protected void UpdateObjectiveRegistration()
        {
            objectives.Filter(o => o != null);

            for (int i = 0; i < _registeredObjectives.Count; i++)
            {
                var objective = _registeredObjectives[i];

                if (!objectives.Contains(objective)) { 
                    objective.Unregister();
                    _registeredObjectives.Remove(objective);
                    i--;
                }
            }

            foreach (QuestObjective objective in objectives)
            {
                if (_registeredObjectives.Contains(objective)) { continue; }

                if (objective.IsRegistered)
                {
                    objective.Unregister();
                }

                objective.Register(this);
                _registeredObjectives.Add(objective);
            }
        }

        public virtual void HandleMessage(IQuestMessage questMessage)
        {
            foreach (var objective in objectives) 
            {
                if (objective.IsRegistered && !objective.IsHidden())
                    objective.OnMessage(questMessage);
            }
        }

        public void CheckCompletion()
        {
            if (State != QuestState.InProgress) {  return; }

            foreach (var objective in objectives)
            {
                if (!objective.IsComplete()) 
                { 
                    return; 
                }
            }

            Complete();
        }

        public void Complete()
        {
            if (State == QuestState.InProgress)
            {
                State = QuestState.Complete;
                Game1.stats.QuestsCompleted++;
                Game1.playSound("questcomplete");
                Game1.dayTimeMoneyBox.questsDirty = true;
                // TODO: Iterate rewards here to claim
            }
        }

        [OnDeserialized]
        internal void OnDeserialized(StreamingContext context)
        {
            objectives.Filter(o => o != null);
            OnDeserialized();
        }

        protected virtual void OnDeserialized() 
        {
        }

        protected virtual void OnCanceled()
        {
        }

        public void OnCancel()
        {
            Manager?.Quests.Remove(this);
            OnCanceled();
        }

        public virtual List<string> GetObjectiveDescriptions()
        {
            return Objectives
                .Where(o => !o.IsHidden())
                .Select(o => o.ShouldShowProgress() 
                    ? $"{ParseAndLocalizeText(o.GetDescription())} ({o.GetCount()}/{o.GetRequiredCount()})" 
                    : ParseAndLocalizeText(o.GetDescription()))
                .ToList();
        }

        public IList<IQuestObjective> GetObjectives()
        {
            return Objectives
                .Where(o => !o.IsHidden())
                .ToList<IQuestObjective>();
        }

        public virtual string ParseAndLocalizeText(string text)
        {
            return TokenParser.ParseText(text.Trim(), customParser: Localize);
        }

        private bool Localize(string[] query, out string replacement, Random random, Farmer player)
        {
            if (query.Length > 0)
            {
                string key = $"Strings\\Quests:{query[0]}";
                replacement = Game1.content.LoadString(key);

                return replacement != key;
            }

            replacement = "";
            return false;
        }

        public virtual string Parse(string rawValue)
        {
            return ParseAndLocalizeText(rawValue);
        }
    }
}
