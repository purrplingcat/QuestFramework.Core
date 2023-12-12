using StardewValley;
using StardewValley.Delegates;
using StardewValley.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static StardewValley.GameStateQuery;

namespace QuestFramework.Internal
{
    internal static class Queries
    {
        /// <summary>
        /// Register QF core's game state queries for <seealso cref="GameStateQuery"/>.
        /// </summary>
        public static void Register()
        {
            var methods = typeof(QueryResolvers).GetMethods(BindingFlags.Static | BindingFlags.Public);

            foreach (MethodInfo method in methods)
            {
                var queryDelegate = (GameStateQueryDelegate)Delegate.CreateDelegate(typeof(GameStateQueryDelegate), method);
                GameStateQuery.Register(method.Name, queryDelegate);
                Logger.Trace($"Registered game state query: {method.Name}");
            }
        }
        public static class QueryResolvers
        {
            public static bool PLAYER_HAS_QUEST(string[] query, GameLocation location, Farmer player, Item targetItem, Item inputItem, Random random)
            {
                if (!ArgUtility.TryGet(query, 1, out var playerKey, out var error) || !ArgUtility.TryGet(query, 2, out var questId, out error))
                {
                    return Helpers.ErrorResult(query, error);
                }
                return Helpers.WithPlayer(player, playerKey, (Farmer target) => target.hasQuest(questId));
            }
        }
    }
}
