using Newtonsoft.Json.Linq;
using QuestFramework.API;

namespace QuestFramework.Framework.Model
{
    internal class QuestMessage<T> : IQuestMessage where T : class
    {
        private readonly T? _message;
        public string Type { get; }

        public QuestMessage(T? message, string name) 
        {
            _message = message;
            Type = name;
        }

        public object? Read() => _message;

        public TMessage? ReadAs<TMessage>() where TMessage : class
        {
            if (_message == null)
            {
                return null;
            }

            if (_message is TMessage message) 
            {
                return message;
            }

            return JObject.FromObject(_message).ToObject<TMessage>();
        }
    }
}
