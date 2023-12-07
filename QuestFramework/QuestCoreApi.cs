using System.Reflection;
using StardewValley;
using StardewModdingAPI;
using QuestFramework.Json;
using QuestFramework.Core;
using QuestFramework.Offering;

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

        public INpcQuestOfferManager NpcQuestOffers => QuestCoreMod.NpcQuestManager;
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
    }
}
