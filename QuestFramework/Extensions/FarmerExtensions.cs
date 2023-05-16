using QuestFramework.Framework;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuestFramework.Extensions
{
    internal static class FarmerExtensions
    {
        public static QuestManager GetQuestManager(this Farmer farmer)
        {
            return QuestManager.Managers[farmer.UniqueMultiplayerID];
        }
    }
}
