using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuestFramework.API
{
    public interface IQuestObjective
    {
        string Id { get; }
        int GetCount();
        int GetRequiredCount();
        void SetCount(int count);
        void IncrementCount(int amount);
        bool IsComplete();
        bool IsHidden();
        string GetDescription();
        bool ShouldShowProgress();
    }
}
