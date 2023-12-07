using QuestFramework.API;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuestFramework.Core
{
    public record QuestMetadata
    {
        public string QualifiedId { get; init; } = "";

        public string LocalId { get; init; } = "";

        public string TypeIdentifier { get; init; } = "";

        public int Seed { get; init; }

        public static QuestMetadata Create(string questId, int? seed = null)
        {
            int splitIndex = questId.IndexOf(')');
            string qualifier = questId[..(splitIndex + 1)];

            return new QuestMetadata()
            {
                QualifiedId = questId,
                LocalId = questId.Replace(qualifier, ""),
                TypeIdentifier = qualifier[1..(qualifier.Length - 1)],
                Seed = seed ?? Game1.random.Next(),
            };
        }
    }
}
