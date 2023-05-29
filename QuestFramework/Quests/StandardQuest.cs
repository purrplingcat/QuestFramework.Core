using Netcode;
using Newtonsoft.Json;
using QuestFramework.API;
using QuestFramework.Framework.Attributes;
using QuestFramework.Quests.Objectives;

namespace QuestFramework.Quests
{
    [CustomQuest("quest")]
    public class StandardQuest : CustomQuest, IHaveObjectives
    {
        [JsonProperty("Objectives")]
        public IList<QuestObjective> Objectives => objectives;

        public StandardQuest() { }

        public StandardQuest(string questId, string questKey = "", string typeDefinitionId = "") : base(questId, questKey, typeDefinitionId)
        {
            Reload();
        }

        protected override void InitNetFields(NetFields netFields)
        {
            base.InitNetFields(netFields);
        }

        public override bool CanBeCancelled()
        {
            throw new NotImplementedException();
        }

        public override int GetDaysLeft()
        {
            throw new NotImplementedException();
        }

        public override string GetDescription()
        {
            throw new NotImplementedException();
        }

        public override int GetMoneyReward()
        {
            throw new NotImplementedException();
        }

        public override string GetName()
        {
            throw new NotImplementedException();
        }

        public override List<string> GetObjectiveDescriptions()
        {
            throw new NotImplementedException();
        }

        public IList<IQuestObjective> GetObjectives()
        {
            throw new NotImplementedException();
        }

        public override bool HasMoneyReward()
        {
            throw new NotImplementedException();
        }

        public override bool HasReward()
        {
            throw new NotImplementedException();
        }

        public override bool IsAccepted()
        {
            // TODO: For testing purposes, dehardcode later
            return true;
        }

        public override bool IsHidden()
        {
            throw new NotImplementedException();
        }

        public override bool IsTimedQuest()
        {
            throw new NotImplementedException();
        }

        public override void MarkAsViewed()
        {
            throw new NotImplementedException();
        }

        public override void OnAccept()
        {
            throw new NotImplementedException();
        }

        public override bool OnLeaveQuestPage()
        {
            throw new NotImplementedException();
        }

        public override void OnMoneyRewardClaimed()
        {
            throw new NotImplementedException();
        }

        public override bool Reload()
        {
            return true;
        }

        public override bool ShouldDisplayAsComplete()
        {
            throw new NotImplementedException();
        }

        public override bool ShouldDisplayAsNew()
        {
            throw new NotImplementedException();
        }
    }
}
