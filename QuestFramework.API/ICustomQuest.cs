using Netcode;
using StardewValley.Quests;

namespace QuestFramework.API
{
    public interface ICustomQuest : IQuest, INetObject<NetFields>
    {
    }
}
