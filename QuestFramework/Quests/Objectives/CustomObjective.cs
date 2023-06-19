using Netcode;
using Newtonsoft.Json;
using QuestFramework.API;
using QuestFramework.Extensions;
using QuestFramework.Framework;
using StardewValley;
using StardewValley.Internal;

namespace QuestFramework.Quests.Objectives
{

    public class CustomObjective : QuestObjective
    {
        public delegate void CustomObjectiveDelegate(IQuestObjective objective, IQuestMessage message, ModDataDictionary data, ICustomQuest quest);

        private readonly NetString handlerMethod = new();
        private readonly NetBool showProgress = new();

        [JsonIgnore]
        public ModDataDictionary ModData { get; } = new ModDataDictionary();

        [JsonProperty("mod_data")]
        [Obsolete("For JSON serialization purposes only! Use ModData property instead.")]
        public IDictionary<string, string> ModDataForSerialization
        { 
            get => ModData.ToDictionary();
            set => ModData.SetFromDictionary(value);
        }

        [JsonProperty("method_name")]
        public string HandlerMethod
        {
            get => handlerMethod.Value; 
            set => handlerMethod.Value = value;
        }

        [JsonProperty("show_progress")]
        public bool ShowProgress
        {
            get => showProgress.Value;
            set => showProgress.Value = value;
        }

        protected override void InitNetFields()
        {
            base.InitNetFields();

            NetFields
                .AddField(handlerMethod, "handlerMethod")
                .AddField(ModData, "modData");
        }

        public override void Load(CustomQuest quest, Dictionary<string, string> data)
        {
            if (data.TryGetValue("ShowProgress", out var rawValue) && rawValue.ToLowerInvariant() == "true")
            {
                showProgress.Value = true;
            }

            foreach (var item in data)
            {
                ModData[item.Key] = item.Value;
            }
        }

        protected override void HandleQuestMessage(IQuestMessage questMessage)
        {
            if (_quest == null) { return; }

            if (StaticDelegateBuilder.TryCreateDelegate<CustomObjectiveDelegate>(HandlerMethod, out var handler, out var error))
            {
                handler(this, questMessage, ModData, _quest);
                return;
            }

            Logger.Warn(error);
        }

        public override bool ShouldShowProgress()
        {
            return ShowProgress;
        }
    }
}
