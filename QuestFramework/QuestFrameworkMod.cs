using Newtonsoft.Json;
using QuestFramework.Extensions;
using QuestFramework.Framework;
using QuestFramework.Framework.Networking;
using StardewValley;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using System.Diagnostics.CodeAnalysis;

namespace QuestFramework
{
    public class QuestFrameworkMod : Mod
    {
        private readonly JsonSerializerSettings _jsonSerializerSettings = new();

        [AllowNull]
        internal static QuestSynchronizer Synchronizer { get; private set; }

        [AllowNull]
        internal static QuestSaveManager SaveManager { get; private set; }

        public static QuestFrameworkConfig Config { get; private set; } = new();

        public override void Entry(IModHelper helper)
        {
            Logger.Setup(Monitor);

            Config = helper.ReadConfig<QuestFrameworkConfig>();
            Synchronizer = new QuestSynchronizer(helper.Events, helper.Multiplayer, helper.Translation, ModManifest);
            SaveManager = new QuestSaveManager(_jsonSerializerSettings, helper.Data, ModManifest);
            
            helper.Events.GameLoop.UpdateTicking += OnGameUpdating;
            helper.Events.GameLoop.SaveLoaded += OnSaveLoaded;
            helper.Events.GameLoop.Saving += OnSaving;
            helper.Events.GameLoop.ReturnedToTitle += OnExitToTitle;
            helper.Events.Input.ButtonPressed += Input_ButtonPressed;
        }

        // TODO: Only for test purposes. Remove it when it's not needed anymore
        private void Input_ButtonPressed(object? sender, ButtonPressedEventArgs e)
        {
            if (Context.IsWorldReady && e.Button == SButton.F5)
            {
                Game1.player.GetQuestManager().AddQuest("test");
            }
        }

        private void OnGameUpdating(object? sender, UpdateTickingEventArgs e)
        {
            if (!Context.IsWorldReady) { return; }

            if (e.IsMultipleOf(Config.UpdateRate))
            {
                QuestManager.Current?.Update();
            }
        }

        private void OnSaving(object? sender, SavingEventArgs e)
        {
            if (!Context.IsMainPlayer) { return; }

            SaveManager.SaveState();
        }

        private void OnSaveLoaded(object? sender, SaveLoadedEventArgs e)
        {
            if (!Context.IsMainPlayer) { return; }

            SaveManager.LoadState();
        }

        private void OnExitToTitle(object? sender, ReturnedToTitleEventArgs e)
        {
            foreach (var manager in QuestManager.Managers.Values)
            {
                if (manager is IDisposable disbosable)
                {
                    disbosable.Dispose();
                }
            }

            QuestManager.Managers.Clear();
            Monitor.Log("Quest Managers were uninitialized", LogLevel.Info);
        }
    }
}
