using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;

using StardewValley;

using StardewModdingAPI;
using StardewModdingAPI.Events;

using QuestFramework.Core;
using QuestFramework.Core.Networking;
using QuestFramework.Core.Patching;
using QuestFramework.Extensions;
using QuestFramework.Internal;
using QuestFramework.Json;
using QuestFramework.Patches;
using QuestFramework.API;
using StardewModdingAPI.Utilities;
using QuestFramework.Menus;

namespace QuestFramework
{
    public class QuestCoreMod : Mod
    {
        private readonly JsonSerializerSettings _jsonSerializerSettings = new();        
        private bool _hold = true;

        [AllowNull]
        internal static QuestSynchronizer Synchronizer { get; private set; }

        [AllowNull]
        internal static QuestSaveManager SaveManager { get; private set; }

        [AllowNull]
        internal static QuestEvents Events { get; private set; }
        
        [AllowNull]
        internal static IReflectionHelper Reflection { get; private set; }

        public static QuestCoreConfig Config { get; private set; } = new();
        public static string ModId { get; private set; } = "";

        public override void Entry(IModHelper helper)
        {
            Logger.Setup(Monitor);

            JsonTypesManager.RegisterTypesFromAssembly(GetType().Assembly);
            EventCommands.RegisterCommands(ModManifest.UniqueID);

            ModId = ModManifest.UniqueID;
            Reflection = helper.Reflection;
            Config = helper.ReadConfig<QuestCoreConfig>();
            Synchronizer = new QuestSynchronizer(this, QuestManager.Managers);
            SaveManager = new QuestSaveManager(_jsonSerializerSettings, helper.Data, ModManifest);
            Events = new QuestEvents();

            HarmonyPatcher.Apply(this, new Patcher[] {
                new FarmerPatcher(),
            });

            helper.Events.GameLoop.DayStarted += OnDayStarted;
            helper.Events.GameLoop.DayEnding += OnDayEnding;
            helper.Events.GameLoop.UpdateTicking += OnGameUpdating;
            helper.Events.GameLoop.SaveLoaded += OnSaveLoaded;
            helper.Events.GameLoop.Saving += OnSaving;
            helper.Events.GameLoop.ReturnedToTitle += OnExitToTitle;
            helper.Events.Input.ButtonPressed += Input_ButtonPressed;
            CustomQuestLog.HookOnQuestLog(helper.Events.Display);
        }

        public override object? GetApi(IModInfo mod)
        {
            return QuestCoreApi.RequestApi(mod.Manifest);
        }

        [EventPriority(EventPriority.High)]
        private void OnDayEnding(object? sender, DayEndingEventArgs e)
        {
            _hold = true;
            QuestManager.Current?.Update();
            FakeOrder.Uninstall();
        }

        [EventPriority(EventPriority.High)]
        private void OnDayStarted(object? sender, DayStartedEventArgs e)
        {
            _hold = false;
            QuestManager.Current?.Update();
            FakeOrder.Install();
        }

        // TODO: Only for test purposes. Remove it when it's not needed anymore
        private void Input_ButtonPressed(object? sender, ButtonPressedEventArgs e)
        {
            if (!Context.IsWorldReady) { return; }
            
            if (e.Button == SButton.F5)
            {
                Game1.player.GetQuestManager()?.AddQuest("test");
            }

            if (e.Button == SButton.F6)
            {
                SaveManager.SaveState();
            }

            if (e.Button == SButton.F8)
            {
                SaveManager.LoadState();
            }
        }

        private void OnGameUpdating(object? sender, UpdateTickingEventArgs e)
        {
            if (_hold || !Context.IsWorldReady) { return; }

            if (e.IsMultipleOf(Config.UpdateRate))
            {
                FakeOrder.Install();
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
            _hold = true;

            foreach (var manager in QuestManager.Managers.Values)
            {
                if (manager is IDisposable disbosable)
                {
                    disbosable.Dispose();
                }
            }

            FakeOrder.CleanUp();
            QuestManager.Managers.Clear();
            Monitor.Log("Quest Managers were uninitialized", LogLevel.Info);
        }
    }
}
