using Netcode;
using StardewValley.Quests;

namespace QuestFramework.API
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
        void HandleEvent(IQuestEvent questMessage);
        bool Reload();
        void Update();
        void CheckCompletion();
    }
}
