using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuestFramework.Core
{
    internal static class Utils
    {
        public static bool IsQfQuestId(string questId)
        {
            if (string.IsNullOrEmpty(questId))
                return false;

            if (questId.StartsWith('#'))
                return true;

            return questId.StartsWith('(')
                && questId.Contains(')');
        }
    }
}
