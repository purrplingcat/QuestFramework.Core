using QuestFramework.Offering;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace QuestFramework.Core
{
    public interface IQuestCore
    {
        public IQuestEvents Events { get; }
        public INpcQuestOfferManager NpcQuestOffers { get; }
        public IQuestManager? GetQuestManager();
        public IQuestManager? GetQuestManager(Farmer player);
        public IQuestManager? GetQuestManager(long playerId);
        public void RegisterTypes(params Type[] types);
        public void RegisterTypes(Assembly assembly);
        public void RegisterQuestProvider(string token, IQuestProvider provider);
    }
}
