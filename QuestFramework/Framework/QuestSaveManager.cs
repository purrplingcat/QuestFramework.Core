using Newtonsoft.Json;
using QuestFramework.Framework.Model;
using StardewModdingAPI;
using StardewValley;

namespace QuestFramework.Framework
{
    internal class QuestSaveManager
    {
        private const string SAVE_KEY = "QuestState";

        private readonly JsonSerializerSettings _jsonSerializerSettings;
        private readonly IDataHelper _data;
        private readonly IManifest _manifest;

        public QuestSaveManager(JsonSerializerSettings jsonSerializerSettings, IDataHelper data, IManifest manifest)
        {
            _jsonSerializerSettings = jsonSerializerSettings;
            _data = data;
            _manifest = manifest;
        }

        public void SaveState()
        {
            var serializer = JsonSerializer.Create(_jsonSerializerSettings);
            var save = new QuestFrameworkState
            {
                Version = _manifest.Version,
                Managers = QuestManager.Managers
                .ToDictionary((m) => m.Key, (m) => m.Value.GetSaveState())
            };

            _data.WriteSaveData(SAVE_KEY, save);
            Game1.CustomData.Any(); // TODO: Only for debug. Remove this later
            Logger.Info($"State successfully saved for {save.Managers.Count} managers");
        }

        public void LoadState()
        {
            QuestFrameworkState save = _data.ReadSaveData<QuestFrameworkState>(SAVE_KEY) ?? new();
            //QuestManager.Managers.Clear();

            foreach (var farmer in Game1.getAllFarmers())
            {
                var manager = new QuestManager(farmer);

                if (save.Managers.TryGetValue(farmer.UniqueMultiplayerID, out var managerState))
                {
                    manager.LoadFromState(managerState);
                }

                QuestManager.Managers.Add(farmer.UniqueMultiplayerID, manager);
                Logger.Debug($"Loaded Quest Manager state for player '{farmer.Name}' ({farmer.UniqueMultiplayerID})");
            }

            Logger.Info($"Successully initialized {QuestManager.Managers.Count} Quest Managers");
        }

        public static void HookOnFarmerAddedOrRemoved()
        {
            if (!Context.IsWorldReady) { return; }

            Game1.netWorldState.Value.farmhandData.OnValueAdded += (long key, Farmer farmer) =>
            {
                if (QuestManager.Managers.ContainsKey(key))
                {
                    QuestManager.Managers.Add(key, new QuestManager(farmer));
                }
            };

            Game1.netWorldState.Value.farmhandData.OnValueRemoved += (long key, Farmer value) =>
            {
                if (QuestManager.Managers.ContainsKey(key))
                {
                    if (QuestManager.Managers[key] is IDisposable disposable)
                    {
                        disposable.Dispose();
                    }

                    QuestManager.Managers.Remove(key);
                }
            };
        }
    }
}
