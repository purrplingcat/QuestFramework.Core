using JsonKnownTypes;
using Netcode;
using Newtonsoft.Json;
using QuestFramework.API;
using QuestFramework.Framework.Converters;
using QuestFramework.Quests.Data;
using StardewValley;

namespace QuestFramework.Quests.Objectives
{
    [JsonConverter(typeof(QuestConverter<QuestObjective>))]
    [JsonTypeInclude(typeof(CollectObjective), "Collect")]
    [JsonTypeInclude(typeof(SlayObjective), "Slay")]
    [JsonTypeInclude(typeof(CustomObjective), "Custom")]
    public abstract class QuestObjective : IQuestObjective, INetObject<NetFields>
    {
        protected bool _complete;
        protected CustomQuest? _quest;
        protected readonly NetString id = new();
        protected readonly NetInt currentCount = new(0);
        protected readonly NetInt requiredCount = new(1);
        protected readonly NetString conditionsQueryString = new();
        protected readonly NetString description = new();
        protected readonly NetStringList requiredObjectives = new();

        [JsonProperty]
        public string Id
        {
            get => id.Value;
            set => id.Value = value;
        }

        [JsonProperty]
        public int CurrentCount
        {
            get => currentCount.Value; 
            set => currentCount.Value = value;
        }

        [JsonProperty]
        public int RequiredCount
        {
            get => requiredCount.Value;
            set => requiredCount.Value = value;
        }

        [JsonProperty]
        public string Description
        {
            get => description.Value;
            set => description.Value = value;
        }

        [JsonProperty]
        public IList<string> RequiredObjectives 
        { 
            get => requiredObjectives;
            set => requiredObjectives.CopyFrom(value); 
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

        protected bool CheckConditions(GameLocation? location = null, Farmer? player = null, Item? targetItem = null, Item? inputItem = null, Random? random = null, HashSet<string>? ignoreQueryKeys = null)
        {
            if (string.IsNullOrWhiteSpace(conditionsQueryString.Value))
            {
                return GameStateQuery.CheckConditions(conditionsQueryString.Value, location, player, targetItem, inputItem, random, ignoreQueryKeys);
            }

            return true;
        }

        protected virtual bool CheckConditions(IQuestMessage questMessage) 
            => CheckConditions();

        public string GetDescription()
        {
            return Description;
        }

        public int GetCount() => CurrentCount;

        public int GetRequiredCount() => RequiredCount;

        public bool IsComplete() => _complete;
        
        public bool IsHidden()
        {
            if (!IsRegistered)
                return true;

            if (_quest != null && RequiredObjectives.Any())
            {
                foreach (var objective in _quest.Objectives)
                {
                    if (RequiredObjectives.Contains(objective.Id) && !objective.IsComplete())
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public virtual void IncrementCount(int amount) => SetCount(CurrentCount + amount);

        public virtual void SetCount(int count)
        {
            int newCount = Math.Max(0, Math.Min(count, RequiredCount));

            if (newCount != CurrentCount)
            {
                CurrentCount = newCount;
            }
        }

        public virtual bool ShouldShowProgress()
        {
            return true;
        }

        public void Register(CustomQuest quest)
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

        public abstract void Load(CustomQuest quest, Dictionary<string, string> data);
        protected abstract void HandleQuestMessage(IQuestMessage questMessage);

        public void OnMessage(IQuestMessage questMessage)
        {
            if (CheckConditions(questMessage))
            {
                HandleQuestMessage(questMessage);
            }
        }

        public static bool TryReadMessage<TMessage>(in IQuestMessage questMessage, out TMessage message, string? type = null) where TMessage : class
        {
            message = default!;

            if (type != null && questMessage.Type != type)
                return false;

            message = questMessage.ReadAs<TMessage>()!;

            return message != null;
        }
    }
}
