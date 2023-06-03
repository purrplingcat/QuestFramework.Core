using Netcode;
using Newtonsoft.Json;
using QuestFramework.API;
using QuestFramework.Framework;
using QuestFramework.Framework.Attributes;
using QuestFramework.Framework.Converters;
using StardewValley;
using StardewValley.Internal;

namespace QuestFramework.Quests.Objectives
{

    public class CustomObjective : QuestObjective, IHaveModData
    {
        public delegate void CustomObjectiveDelegate(IQuestObjective objective, IQuestMessage message, ModDataDictionary data, ICustomQuest quest);

        private readonly NetString handlerMethod = new();

        [JsonIgnore]
        public ModDataDictionary modData { get; } = new ModDataDictionary();

        [JsonProperty("ModData"), JsonConverter(typeof(ModDataConverter))]
        public ModDataDictionary modDataForSerialization
        { 
            get => modData.GetForSerialization();
            set => modData.SetFromSerialization(value);
        }

        [JsonProperty("MethodName")]
        public string HandlerMethod
        {
            get => handlerMethod.Value; 
            set => handlerMethod.Value = value;
        }

        protected override void InitNetFields()
        {
            base.InitNetFields();

            NetFields
                .AddField(handlerMethod, "handlerMethod")
                .AddField(modData, "modData");
        }

        public override void Load(ICustomQuest quest, Dictionary<string, string> data)
        {
            foreach (var item in data)
            {
                modData[item.Key] = item.Value;
            }
        }

        protected override void HandleMessage(IQuestMessage questMessage)
        {
            if (_quest == null) { return; }

            if (StaticDelegateBuilder.TryCreateDelegate<CustomObjectiveDelegate>(HandlerMethod, out var handler, out var error))
            {
                handler(this, questMessage, modData, _quest);
                return;
            }

            Logger.Warn(error);
        }
    }
}
