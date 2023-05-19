using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuestFramework.Framework
{
    internal static class Utils
    {
        public static bool IsQfQuestId(string dirtyQuestId, out string qfQuestId)
        {
            const string PREFIX = "QF:";

            if (dirtyQuestId.StartsWith(PREFIX))
            {
                qfQuestId = dirtyQuestId[..PREFIX.Length];
                return true;
            }

            qfQuestId = "";
            return false;
        }
    }
}
