using Netcode;
using Newtonsoft.Json;
using QuestFramework.API;
using QuestFramework.Quests.Objectives;

namespace QuestFramework.Quests
{
    public abstract class CustomQuest : ICustomQuest
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
        protected readonly NetEnum<QuestState> state = new(QuestState.InProgress);
        protected NetObjectList<QuestObjective> objectives = new();
        protected bool _objectivesRegistrationDirty;

        [JsonProperty("Id")]
        public string Id
        {
            get => id.Value;
            set => id.Value = value;
        }

        [JsonProperty("QuestKey")]
        public string QuestKey 
        { 
            get => questKey.Value; 
            set => questKey.Value = value; 
        }

        [JsonProperty("TypeDefinitionId")]
        public string TypeDefinitionId 
        { 
            get => typeDefinitionId.Value;
            set => typeDefinitionId.Value = value;
        }

        [JsonProperty("State")]
        public QuestState State
        {
            get => state.Value;
            set => state.Value = value;
        }

        [JsonIgnore]
        public IQuestManager? Manager { get; private set; }

        [JsonIgnore]
        public NetFields NetFields { get; }

        public CustomQuest() 
        {
            NetFields = new NetFields(NetFields.GetNameForInstance(this));
            InitNetFields(NetFields);
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

        public abstract string GetName();
        public abstract bool IsAccepted();
        public abstract string GetDescription();
        public abstract List<string> GetObjectiveDescriptions();
        public abstract bool CanBeCancelled();
        public abstract void MarkAsViewed();
        public abstract bool ShouldDisplayAsNew();
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

        public virtual void OnRemoved()
        {
            Manager = null;
        }

        protected void UpdateObjectiveRegistration()
        {
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
        }
    }
}
