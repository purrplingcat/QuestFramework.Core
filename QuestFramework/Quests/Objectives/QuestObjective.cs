using Netcode;
using Newtonsoft.Json;
using QuestFramework.API;
using StardewValley;

namespace QuestFramework.Quests.Objectives
{
    public abstract class QuestObjective : IQuestObjective, INetObject<NetFields>
    {
        protected bool _complete;
        protected ICustomQuest? _quest;
        protected readonly NetInt currentCount = new(0);
        protected readonly NetInt requiredCount = new(1);
        protected readonly NetString conditionsQuery = new();

        [JsonProperty("CurrentCount")]
        public int CurrentCount
        {
            get => currentCount.Value; 
            set => currentCount.Value = value;
        }

        [JsonProperty("RequiredCount")]
        public int RequiredCount
        {
            get => requiredCount.Value;
            set => requiredCount.Value = value;
        }

        [JsonIgnore]
        public bool IsRegistered => _quest != null;

        public QuestObjective()
        {
            NetFields = new NetFields(NetFields.GetNameForInstance(this));
            InitNetFields();
        }

        [JsonIgnore]
        public NetFields NetFields { get; }

        protected virtual void InitNetFields()
        {
            NetFields.SetOwner(this)
                .AddField(currentCount, "currentCount")
                .AddField(requiredCount, "requiredCount");

            currentCount.fieldChangeVisibleEvent += OnCurrentCountChanged;
        }

        private void OnCurrentCountChanged(NetInt field, int oldValue, int newValue)
        {
            if (!Utility.ShouldIgnoreValueChangeCallback())
            {
                CheckCompletion();
            }
        }

        public virtual void CheckCompletion(bool playSound = true)
        {
            if (IsRegistered) { return; }

            bool wasJustCompleted = false;

            if (GetCount() >= GetRequiredCount()) 
            {
                if (!_complete)
                {
                    wasJustCompleted = true;
                }
                _complete = true;
            }

            if (_quest != null) 
            {
                _quest.CheckCompletion();

                if (wasJustCompleted && playSound) 
                {
                    Game1.playSound("jingle1");
                }
            }
        }

        protected bool CheckConditions(GameLocation? location = null, Farmer? player = null, Item? item = null, Random? random = null, HashSet<string>? ignoreQueryKeys = null)
        {
            if (string.IsNullOrWhiteSpace(conditionsQuery.Value))
            {
                return GameStateQuery.CheckConditions(conditionsQuery.Value, location, player, item, random, ignoreQueryKeys);
            }

            return true;
        }

        public string GetDescription()
        {
            throw new NotImplementedException();
        }

        public int GetCount() => CurrentCount;

        public int GetRequiredCount() => RequiredCount;

        public bool IsComplete() => _complete;

        public void IncrementCount(int amount) => SetCount(CurrentCount + amount);

        public void SetCount(int count)
        {
            int newCount = Math.Max(0, Math.Min(count, RequiredCount));

            if (newCount != CurrentCount)
            {
                CurrentCount = newCount;
            }
        }

        public virtual bool ShouldShowProgress()
        {
            return false;
        }

        public void Register(ICustomQuest quest)
        {
            _quest = quest;
            OnRegister();
            CheckCompletion(playSound: false);
        }

        public void Unregister()
        {
            _quest = null;
            OnUnregister();
        }

        protected virtual void OnRegister()
        {
        }

        protected virtual void OnUnregister()
        {
        }

        protected abstract void HandleMessage(IQuestMessage questMessage);

        public void OnMessage(IQuestMessage questMessage)
        {
            if (CheckConditions())
            {
                HandleMessage(questMessage);
            }
        }
    }
}
