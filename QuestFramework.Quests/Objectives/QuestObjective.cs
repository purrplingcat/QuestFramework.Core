using Netcode;
using Newtonsoft.Json;
using QuestFramework.API;
using QuestFramework.API.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuestFramework.Quests.Objectives
{
    [QuestObjective("objective")]
    public class QuestObjective : IQuestObjective, INetObject<NetFields>
    {
        protected readonly NetInt currentCount = new(0);
        protected readonly NetInt requiredCount = new(1);

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

        public void IncrementCount(int amount)
        {
            SetCount(CurrentCount + amount);
        }

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
    }
}
