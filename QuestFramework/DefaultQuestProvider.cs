using QuestFramework.API;
using QuestFramework.Quests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuestFramework
{
    internal class DefaultQuestProvider : IQuestProvider
    {
        public ICustomQuest CreateQuest(string questId)
        {
            return new CustomQuest();
        }
    }
}
