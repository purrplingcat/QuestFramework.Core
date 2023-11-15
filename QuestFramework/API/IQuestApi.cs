using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuestFramework.API
{
    public interface IQuestApi
    {
        public IQuestEvents Events { get; }
        public IQuestManager? GetQuestManager();
        public IQuestManager? GetQuestManager(Farmer player);
        public IQuestManager? GetQuestManager(long playerId);
        public void RegisterTypes(params Type[] types);
        public void RegisterQuestProvider(string token, IQuestProvider provider);
    }
}
