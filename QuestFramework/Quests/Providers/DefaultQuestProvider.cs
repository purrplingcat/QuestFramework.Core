using QuestFramework.API;
using QuestFramework.Framework.Exceptions;
using QuestFramework.Quests.Data;
using QuestFramework.Quests.Objectives;
using StardewValley;
using static QuestFramework.Quests.CustomQuest;

namespace QuestFramework.Quests.Providers
{
    public class DefaultQuestProvider : IQuestProvider
    {
        internal static string AssetName => $"Mods/{QuestFrameworkMod.ModId}/Quests";

        private static TBase CreateInstance<TBase>(string typeName, string? fallbackType = null)
        {
            var baseType = typeof(TBase);
            var type = Type.GetType(typeName);

            if (type == null && !string.IsNullOrWhiteSpace(fallbackType))
            {
                type = Type.GetType(fallbackType);
            }

            if (type == null || !type.IsSubclassOf(baseType)) 
            {
                throw new QuestException($"Unable to create instance of class type '{typeName}' because it's not subclass of '{baseType.FullName}'");
            }

            var quest = (TBase?)Activator.CreateInstance(type);
            return quest ?? throw new QuestException($"Unable to create instance of class type '{typeName}'");
        }

        public ICustomQuest? CreateQuest(IQuestMetadata metadata)
        {
            var quests = Game1.content.Load<Dictionary<string, CustomQuestData>>(AssetName);

            if (!quests.TryGetValue(metadata.LocalId, out var questData))
            {
                return null;
            }

            var quest = CreateInstance<CustomQuest>($"QuestFramework.Quests.{questData.Type}Quest", questData.Type);

            quest.Id = metadata.QualifiedId;
            quest.QuestKey = metadata.LocalId;
            quest.TypeDefinitionId = metadata.TypeIdentifier;
            quest.Name = questData.Name;
            quest.Description = questData.Description;
            quest.ShowNew = true;
            quest.State = QuestState.InProgress;

            foreach ((var key, var data) in questData.Objectives)
            {
                var objective = CreateInstance<QuestObjective>($"QuestFramework.Quests.Objectives.{data.Type}Objective", data.Type);
                
                objective.Id = key;
                objective.Description = data.Description;
                objective.RequiredCount = data.RequiredCount;
                objective.Load(quest, data.Data);

                quest.Objectives.Add(objective);
            }

            return quest;
        }
    }
}
