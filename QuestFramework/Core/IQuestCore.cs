using StardewValley;
using System.Reflection;

namespace QuestFramework.Core
{
    public enum QuestMark
    {
        None,
        Default,
        Exclamation,
        ExclamationBlue,
        ExclamationGreen,
        ExclamationBig,
        Question,
        Arrow
    }

    public interface IQuestCore
    {
        IQuestEvents Events { get; }
        IQuestManager? GetQuestManager();
        IQuestManager? GetQuestManager(Farmer player);
        IQuestManager? GetQuestManager(long playerId);
        void RegisterTypes(params Type[] types);
        void RegisterTypes(Assembly assembly);
        void RegisterQuestProvider(string token, IQuestProvider provider);
        void StackQuest(string npcName, string questId, QuestMark marker, string? dialogueKey = null);
        void StackQuest(NPC npc, string questId, QuestMark marker, string? dialogueKey = null);
        void OfferQuestNow(NPC npc, Farmer player);
        void OfferQuestNow(string npcName, Farmer player);
    }
}
