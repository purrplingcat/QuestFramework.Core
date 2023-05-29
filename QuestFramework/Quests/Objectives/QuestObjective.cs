using Netcode;
using Newtonsoft.Json;
using QuestFramework.API;
using QuestFramework.Framework.Attributes;
using StardewValley;

namespace QuestFramework.Quests.Objectives
{
    [QuestObjective("objective")]
    public class QuestObjective : IQuestObjective, INetObject<NetFields>
    {
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

        public bool IsComplete()
        {
            throw new NotImplementedException();
        }

        public void IncrementCount(int amount) => SetCount(CurrentCount + amount);

        public void SetCount(int count)
        {
            int newCount = Math.Max(0, Math.Min(count, RequiredCount));

            if (newCount != CurrentCount)
            {
                CurrentCount = newCount;
            }
        }

        public bool ShouldShowProgress()
        {
            throw new NotImplementedException();
        }

        public void Register(ICustomQuest quest)
        {
            _quest = quest;
            OnRegister();
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
    }
}
