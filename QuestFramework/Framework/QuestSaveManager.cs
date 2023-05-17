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
            Game1.CustomData.Any();
            Logger.Info($"State successfully saved for {save.Managers.Count} managers");
        }

        public void LoadState()
        {
            QuestFrameworkState save = _data.ReadSaveData<QuestFrameworkState>(SAVE_KEY) ?? new();
            QuestManager.Managers.Clear();

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
    }
}
