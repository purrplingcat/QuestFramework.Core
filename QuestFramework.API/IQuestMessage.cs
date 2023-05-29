namespace QuestFramework.API
{
    public interface IQuestMessage
    {
        string Type { get; }
        object? Read();
        TMessage? ReadAs<TMessage>() where TMessage : class;
    }
}
