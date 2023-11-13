using StardewValley;
using StardewValley.Mods;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuestFramework.Extensions
{
    public static class ModDataExtension
    {
        public static IDictionary<string, string> ToDictionary(this ModDataDictionary modData)
        {
            var dict = new Dictionary<string, string>();

            foreach ( var key in modData.Keys )
            {
                dict[key] = modData[key];
            }

            return dict;
        }

        public static void SetFromDictionary(this ModDataDictionary modData, IDictionary<string, string> source)
        {
            modData.Clear();

            foreach (var key in source.Keys )
            {
                modData[key] = source[key];
            }
        }
    }
}
