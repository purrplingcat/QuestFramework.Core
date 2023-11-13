namespace QuestFramework.API
{
    public interface IQuestEvent
    {
        string Type { get; }
        object? Message { get; }
        TMessage? Cast<TMessage>() where TMessage : class;
    }
}
