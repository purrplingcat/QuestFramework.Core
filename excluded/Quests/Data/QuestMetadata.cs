using QuestFramework.API;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuestFramework.Quests.Data
{
    internal record QuestMetadata : IQuestMetadata
    {
        public string QualifiedId { get; init; } = "";

        public string LocalId { get; init; } = "";

        public string TypeIdentifier { get; init; } = "";

        public int Seed { get; init; }
    }
}
