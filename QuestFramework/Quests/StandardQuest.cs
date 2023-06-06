using JsonKnownTypes;
using Netcode;
using QuestFramework.API;

namespace QuestFramework.Quests
{
    [JsonType("Standard")]
    public class StandardQuest : CustomQuest, IHaveObjectives
    {
        public StandardQuest() { }

        public StandardQuest(string questId, string questKey = "", string typeDefinitionId = "") : base(questId, questKey, typeDefinitionId)
        {
        }

        protected override void InitNetFields(NetFields netFields)
        {
            base.InitNetFields(netFields);
        }

        public override bool CanBeCancelled()
        {
            return false;
        }

        public override int GetDaysLeft()
        {
            return 0;
        }

        public override int GetMoneyReward()
        {
            return 0;
        }

        public override bool HasMoneyReward()
        {
            return GetMoneyReward() > 0;
        }

        public override bool HasReward()
        {
            return false;
        }

        public override bool IsAccepted()
        {
            // TODO: For testing purposes, dehardcode later
            return true;
        }

        public override bool IsHidden()
        {
            return false;
        }

        public override bool IsTimedQuest()
        {
            return false; // Testing purposes only
        }

        public override void MarkAsViewed()
        {
            ShowNew = false;
        }

        public override void OnAccept()
        {
            
        }

        public override bool OnLeaveQuestPage()
        {
            return false;
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
            return false; // TODO: Testing purposes only
        }

        protected override void Initialize()
        {
        }
    }
}
