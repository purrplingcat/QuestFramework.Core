using Newtonsoft.Json.Linq;
using QuestFramework.API;

namespace QuestFramework.Core.Model
{
    internal class QuestEvent<T> : IQuestEvent where T : class
    {
        private readonly T? _message;
        public string Type { get; }

        public QuestEvent(T? message, string name) 
        {
            _message = message;
            Type = name;
        }

        public object? Message => _message;

        public TMessage? Cast<TMessage>() where TMessage : class
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
