namespace QuestFramework.Quests
{
    public class StandardQuest : CustomQuest
    {
        public StandardQuest() { }

        public StandardQuest(string questId, string questKey = "", string typeDefinitionId = "") : base(questId, questKey, typeDefinitionId)
        {
            Reload();
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

        public override void Update()
        {
            // TODO: Implement later
        }
    }
}
