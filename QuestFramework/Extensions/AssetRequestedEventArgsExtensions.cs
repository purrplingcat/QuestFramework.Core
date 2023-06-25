using StardewModdingAPI.Events;

namespace QuestFramework.Extensions
{
    public static class AssetRequestedEventArgsExtensions
    {
        public static void ProvideDataSet<TData>(this AssetRequestedEventArgs e, string assetName) where TData : class
        {
            if (e.NameWithoutLocale.IsEquivalentTo(assetName))
            {
                e.LoadFrom(() => new Dictionary<string, TData>(), AssetLoadPriority.Low);
            }
        }
    }
}
