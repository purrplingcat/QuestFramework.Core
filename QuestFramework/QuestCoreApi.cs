using System.Reflection;
using StardewValley;
using StardewModdingAPI;
using QuestFramework.Json;
using QuestFramework.Core;
using QuestFramework.Offering;
using QuestFramework.Internal;

namespace QuestFramework
{
    public class QuestCoreApi : IQuestCore
    {
        private static readonly Dictionary<string, QuestCoreApi> _apiCache = new();

        private QuestCoreApi(IManifest manifest)
        {
            ModManifest = manifest;
        }

        public IManifest ModManifest { get; }

        public IQuestEvents Events => QuestCoreMod.Events;

        internal NpcQuestManager NpcQuestOffers => QuestCoreMod.NpcQuestManager;
        public IQuestManager? GetQuestManager()
        {
            return QuestManager.Current;
        }

        public IQuestManager? GetQuestManager(Farmer player)
        {
            return GetQuestManager(player.UniqueMultiplayerID);
        }

        public IQuestManager? GetQuestManager(long playerId)
        {
            return QuestManager.Managers.TryGetValue(playerId, out var manager) 
                ? manager 
                : null;
        }

        public void RegisterTypes(params Type[] types)
        {
            JsonTypesManager.RegisterTypes(types);
        }

        public void RegisterTypes(Assembly assembly)
        {
            JsonTypesManager.RegisterTypesFromAssembly(assembly);
        }

        public void RegisterQuestProvider(string token, IQuestProvider provider)
        {
            QuestManager.Providers.Add(token, provider);
        }

        internal static QuestCoreApi RequestApi(IManifest requester)
        {
            if (!_apiCache.TryGetValue(requester.UniqueID, out var api))
            {
                api = new QuestCoreApi(requester);
                _apiCache.Add(api.ModManifest.UniqueID, api);
            }

            return api;
        }

        public void ClearQuestStack()
        {
            throw new NotImplementedException();
        }

        public void ClearQuestStack(string npcName)
        {
            NpcQuestOffers.ClearQuestOffers(npcName);
        }

        public void OfferQuestNow(NPC npc, Farmer player)
        {
            NpcQuestOffers.OfferQuestToPlayer(npc, player);
        }

        public void StackQuest(string npcName, string questId, QuestMark marker, string? dialogueKey = null)
        {
            NpcQuestOffers.AddQuestOffer(npcName, new NpcQuestOffer(questId, marker, npcName, dialogueKey));
        }

        public void StackQuest(NPC npc, string questId, QuestMark marker, string? dialogueKey = null)
        {
            StackQuest(npc.Name, questId, marker, dialogueKey);
        }

        public void ClearQuestStack(NPC npc)
        {
            NpcQuestOffers.ClearQuestOffers(npc.Name);
        }

        public void OfferQuestNow(string npcName, Farmer player)
        {
            NPC npc = Game1.getCharacterFromName(npcName);

            if (npc != null)
            {
                OfferQuestNow(npc, player);
                return;
            }

            Logger.Warn($"[{ModManifest.Name}] Unable to offer a quest: No NPC '{npcName}' was found.");
        }
    }
}
