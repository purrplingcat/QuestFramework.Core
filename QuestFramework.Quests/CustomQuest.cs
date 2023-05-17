using Netcode;
using Newtonsoft.Json;
using QuestFramework.API;

namespace QuestFramework.Quests
{
    public abstract class CustomQuest : ICustomQuest
    {
        protected readonly NetString id = new();
        protected readonly NetString questKey = new("");
        protected readonly NetString typeDefinitionId = new("");

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
                .AddField(typeDefinitionId, "typeDefinitionId");
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

        public abstract void Update();

        public virtual void OnAdd(IQuestManager manager)
        {
            Manager = manager;
        }

        public virtual void OnRemoved()
        {
            Manager = null;
        }
    }
}
