using Netcode;
using QuestFramework.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuestFramework.Quests
{
    public class CustomQuest : ICustomQuest
    {
        public NetFields NetFields { get; }

        public CustomQuest() 
        {
            NetFields = new NetFields(NetFields.GetNameForInstance(this));
            InitNetFields(NetFields);
        }

        protected void InitNetFields(NetFields netFields)
        {
            netFields.SetOwner(this);
        }

        public bool CanBeCancelled()
        {
            throw new NotImplementedException();
        }

        public int GetDaysLeft()
        {
            throw new NotImplementedException();
        }

        public string GetDescription()
        {
            throw new NotImplementedException();
        }

        public int GetMoneyReward()
        {
            throw new NotImplementedException();
        }

        public string GetName()
        {
            throw new NotImplementedException();
        }

        public List<string> GetObjectiveDescriptions()
        {
            throw new NotImplementedException();
        }

        public bool HasMoneyReward()
        {
            throw new NotImplementedException();
        }

        public bool HasReward()
        {
            throw new NotImplementedException();
        }

        public bool IsHidden()
        {
            throw new NotImplementedException();
        }

        public bool IsTimedQuest()
        {
            throw new NotImplementedException();
        }

        public void MarkAsViewed()
        {
            throw new NotImplementedException();
        }

        public bool OnLeaveQuestPage()
        {
            throw new NotImplementedException();
        }

        public void OnMoneyRewardClaimed()
        {
            throw new NotImplementedException();
        }

        public bool ShouldDisplayAsComplete()
        {
            throw new NotImplementedException();
        }

        public bool ShouldDisplayAsNew()
        {
            throw new NotImplementedException();
        }
    }
}
