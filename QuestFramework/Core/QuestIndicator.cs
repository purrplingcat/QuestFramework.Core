using StardewModdingAPI.Utilities;

namespace QuestFramework.Core
{
    public enum QuestMark
    {
        None,
        Default,
        Exclamation,
        ExclamationBlue,
        ExclamationGreen,
        ExclamationBig,
        Question,
        Arrow
    }

    public class QuestIndicator
    {
        protected readonly Dictionary<string, QuestMark> sources = new();

        public string? CurrentSource { get; private set; }
        public QuestMark CurrentMark { get; private set; }

        public bool Visible => sources.Count > 0;

        public void Set(string id, QuestMark type = QuestMark.Default)
        {
            sources[id] = type;
            UpdateMark();
        }

        public void Clear(string id) 
        { 
            sources.Remove(id);
            UpdateMark();
        }

        public void Clear()
        {
            sources.Clear();
            UpdateMark();
        }

        public bool IsSet(string id)
        {
            return sources.ContainsKey(id);
        }

        protected virtual void UpdateMark()
        {
            var source = sources.LastOrDefault();

            CurrentSource = source.Key;
            CurrentMark = source.Value;
        }
    }
}
