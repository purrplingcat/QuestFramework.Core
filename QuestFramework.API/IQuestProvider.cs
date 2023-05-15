using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuestFramework.API
{
    public interface IQuestProvider
    {
        ICustomQuest CreateQuest(string questId);
    }
}
