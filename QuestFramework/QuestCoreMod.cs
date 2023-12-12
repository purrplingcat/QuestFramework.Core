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
using QuestFramework.Menus;
using QuestFramework.Offering;

namespace QuestFramework
{
    internal class QuestCoreMod : Mod
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

        [AllowNull]
        internal static QuestIndicatorManager IndicatorManager { get; private set; }

        [AllowNull]
        internal static NpcQuestManager NpcQuestManager { get; private set; }

        public static QuestCoreConfig Config { get; private set; } = new();

        public override void Entry(IModHelper helper)
        {
            Logger.Setup(Monitor);

            JsonTypesManager.RegisterTypesFromAssembly(GetType().Assembly);
            EventCommands.RegisterCommands(ModManifest.UniqueID);
            Queries.Register();

            Reflection = helper.Reflection;
            Config = helper.ReadConfig<QuestCoreConfig>();
            Synchronizer = new QuestSynchronizer(this, QuestManager.Managers);
            SaveManager = new QuestSaveManager(_jsonSerializerSettings, helper.Data, ModManifest);
            IndicatorManager = new QuestIndicatorManager(helper.Events.Display);
            NpcQuestManager = new NpcQuestManager(IndicatorManager);
            Events = new QuestEvents();

            HarmonyPatcher.Apply(this, new Patcher[] {
                new FarmerPatcher(),
                new NpcPatcher (),
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
            NpcQuestManager.ClearQuestOffers();
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
            
            if (e.Button == SButton.F5 && Game1.currentLocation != null)
            {
                foreach (var npc in Game1.currentLocation.characters)
                {
                    if (npc == null || !npc.isVillager()) continue;

                    var marker = (QuestMark)Game1.random.Next(1, Enum.GetValues(typeof(QuestMark)).Length);
                    NpcQuestManager.AddQuestOffer(npc.Name, new NpcQuestOffer("(Q)test", marker, npc.Name));
                }
            }

            if (e.Button == SButton.F6)
            {
                NpcQuestManager.ClearQuestOffers();
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
            NpcQuestManager.Reset();
            Monitor.Log("Quest Managers were uninitialized", LogLevel.Info);
        }
    }
}
