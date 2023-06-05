﻿using Netcode;
using Newtonsoft.Json;
using QuestFramework.API;
using QuestFramework.Quests.Objectives;
using StardewValley;
using System.Runtime.Serialization;

namespace QuestFramework.Quests
{
    public abstract class CustomQuest : ICustomQuest, IDisposable
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
        protected readonly NetObjectList<QuestObjective> objectives = new();
        protected readonly NetBool showNew = new();
        protected bool _objectivesRegistrationDirty;
        private bool _disposedValue;

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

        [JsonProperty("ShowNew")]
        public bool ShowNew
        {
            get => showNew.Value;
            set => showNew.Value = value;
        }

        [JsonIgnore]
        public IQuestManager? Manager { get; private set; }

        [JsonIgnore]
        public NetFields NetFields { get; }

        [JsonProperty("Objectives")]
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

        public abstract string GetName();
        public abstract bool IsAccepted();
        public abstract string GetDescription();
        public abstract List<string> GetObjectiveDescriptions();
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

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                _disposedValue = true;
            }
        }

        // ~CustomQuest()
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
