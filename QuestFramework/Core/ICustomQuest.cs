using Netcode;
using StardewValley.Quests;

namespace QuestFramework.Core
{
    public interface ICustomQuest : IQuest, INetObject<NetFields>
    {
        string Id { get; set; }
        IQuestManager? Manager { get; }

        bool IsAccepted();
        void OnAccept();
        void OnAdd(IQuestManager manager);
        void OnRemove();
        void OnCancel();
        bool Reload();
        void Update();
        void CheckCompletion();
    }
}
