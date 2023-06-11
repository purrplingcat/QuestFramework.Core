using Netcode;
using Newtonsoft.Json;
using QuestFramework.API;
using QuestFramework.Quests.Data;
using StardewValley.Monsters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuestFramework.Quests.Objectives
{
    public class SlayObjective : QuestObjective
    {
        protected readonly NetStringList targetNames = new();

        [JsonProperty]
        public IList<string> TargetNames
        {
            get => targetNames;
            set => targetNames.CopyFrom(value);
        }

        public override void Load(CustomQuest quest, Dictionary<string, string> data)
        {
            if (data.TryGetValue("TargetName", out var rawValue))
            {
                string[] array = quest.Parse(rawValue).Split(',');
                foreach (string target in array)
                {
                    targetNames.Add(target.Trim());
                }
            }
        }

        protected override void HandleQuestMessage(IQuestMessage questMessage)
        {
            if (TryReadMessage<MonsterMessage>(questMessage, out var monsterMsg, "MonsterSlain"))
                return;

            foreach (string target in targetNames)
            {
                if (monsterMsg.Monster.Name.Contains(target))
                {
                    IncrementCount(1);
                    return;
                }
            }
        }
    }
}
