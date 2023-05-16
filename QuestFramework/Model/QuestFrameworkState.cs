﻿using StardewModdingAPI;

namespace QuestFramework.Model
{
    internal class QuestFrameworkState
    {
        public ISemanticVersion? Version { get; set; }
        public Dictionary<long, QuestManagerState> Managers { get; set; } = new();
    }
}
