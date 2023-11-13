using Netcode;
using Newtonsoft.Json;
using QuestFramework.API;
using QuestFramework.Quests.Data;
using StardewValley;

namespace QuestFramework.Quests.Objectives
{
    public class CollectObjective : QuestObjective
    {
        protected readonly NetStringList acceptableContextTags = new();

        [JsonProperty]
        public IList<string> AcceptableContextTags
        {
            get => acceptableContextTags;
            set => acceptableContextTags.CopyFrom(value);
        }

        public override void Load(CustomQuest quest, Dictionary<string, string> data)
        {
            if (data.TryGetValue("AcceptedContextTags", out var rawValue))
            {
                foreach (var tag in rawValue.Split(','))
                {
                    acceptableContextTags.Add(tag.Trim());
                }
            }
        }

        protected override void HandleQuestMessage(IQuestMessage questMessage)
        {
            if (!TryReadMessage<ItemMessage>(questMessage, out var collected, "ItemCollected"))
                return;

            foreach (var tag in acceptableContextTags)
            {
                if (!ItemContextTagManager.DoAnyTagsMatch(tag.Split('/'), collected.Item.GetContextTags()))
                {
                    return;
                }
            }

            IncrementCount(collected.Item.Stack);
        }
    }
}
