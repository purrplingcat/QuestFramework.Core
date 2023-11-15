using System;

namespace QuestFramework.Json.Exceptions
{
    public class JsonKnownTypesException : Exception
    {
        public JsonKnownTypesException(string message)
            : base(message)
        { }
    }
}
