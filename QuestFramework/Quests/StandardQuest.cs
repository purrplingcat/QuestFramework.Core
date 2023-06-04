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
            return questKey.Value;
        }

        public override List<string> GetObjectiveDescriptions()
        {
            return Objectives
                .Select(o => o.ShouldShowProgress() 
                    ? $"{o.GetDescription()} ({o.GetCount()}/{o.GetRequiredCount()})" 
                    : o.GetDescription())
                .ToList();
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
            return false;
        }

        public override bool IsTimedQuest()
        {
            return false; // Testing purposes only
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
            return false; // TODO: Testing purposes only
        }
    }
}
